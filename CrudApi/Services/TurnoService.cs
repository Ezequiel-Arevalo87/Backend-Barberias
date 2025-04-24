using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Models;
using CrudApi.Notifications;
using Microsoft.EntityFrameworkCore;
using System.Linq;

public class TurnoService : ITurnoService
{
    private readonly Notifications _notificationsService;
    private readonly ApplicationDbContext _context;

    public TurnoService(ApplicationDbContext context, Notifications notificationsService)
    {
        _context = context;
        _notificationsService = notificationsService;
    }

    public async Task<List<TurnoDTO>> ObtenerTurnosAsync(int? barberoId)
    {
        var query = _context.Turnos
            .Include(t => t.Cliente).ThenInclude(c => c.Usuario)
            .Include(t => t.Servicio)
            .AsQueryable();

        if (barberoId.HasValue)
        {
            query = query.Where(t => t.BarberoId == barberoId.Value);
        }

        var turnos = await query.ToListAsync();
        return turnos.Select(t => MapTurnoToDTO(t)).ToList();
    }

    public async Task<TurnoDTO> CrearTurnoAsync(TurnoCreateDTO turnoCreateDTO)
    {
        //var fechaColombia = turnoCreateDTO.FechaHoraInicio;
        var duracion = TimeSpan.FromMinutes(turnoCreateDTO.Duracion);
        var fechaColombia = DateTime.SpecifyKind(turnoCreateDTO.FechaHoraInicio, DateTimeKind.Local);


        var turno = new Turno
        {
            Fecha = fechaColombia,
            BarberoId = turnoCreateDTO.BarberoId,
            ClienteId = turnoCreateDTO.ClienteId,
            ServicioId = turnoCreateDTO.ServicioId,
            FechaHoraInicio = fechaColombia,
            HoraFin = fechaColombia.Add(duracion), // ✅ Esta línea es la clave
            Duracion = duracion,
            Estado = EstadoTurno.Pendiente
        };

        _context.Turnos.Add(turno);
        await _context.SaveChangesAsync();

        return await NotificarTurnoAsync(new TurnoDTO { Id = turno.Id });
    }


    public async Task<TurnoDTO> NotificarTurnoAsync(TurnoDTO turnoInput)
    {
        var turno = await _context.Turnos
            .Include(t => t.Cliente).ThenInclude(c => c.Usuario)
            .Include(t => t.Barbero).ThenInclude(b => b.Usuario)
            .Include(t => t.Servicio)
            .FirstOrDefaultAsync(t => t.Id == turnoInput.Id);

        if (turno == null)
            throw new Exception("❌ No se encontró el turno.");

        if (turno.Notificado)
        {
            Console.WriteLine($"⛔ Turno {turno.Id} ya fue notificado.");
            return MapTurnoToDTO(turno);
        }

        var turnoDTO = MapTurnoToDTO(turno);

        if (!string.IsNullOrWhiteSpace(turno.Cliente?.NotificationToken))
            await _notificationsService.SendNotificationAsync(turno.Cliente.NotificationToken, turnoDTO);

        if (!string.IsNullOrWhiteSpace(turno.Barbero?.NotificationToken))
            await _notificationsService.SendNotificationAsync(turno.Barbero.NotificationToken, turnoDTO);

        turno.Notificado = true;
        await _context.SaveChangesAsync();

        return turnoDTO;
    }

    public async Task<bool> CancelarTurnoAsync(CancelarTurnoDTO dto)
    {
        var turno = await _context.Turnos
            .Include(t => t.Cliente).ThenInclude(c => c.Usuario)
            .Include(t => t.Barbero).ThenInclude(b => b.Usuario)
            .Include(t => t.Servicio)
            .FirstOrDefaultAsync(t => t.Id == dto.TurnoId);

        if (turno == null) return false;

        var minutosRestantes = (turno.FechaHoraInicio - DateTime.Now).TotalMinutes;

        if (dto.Rol == "Cliente")
        {
            turno.Estado = minutosRestantes >= 20 ? EstadoTurno.Disponible : EstadoTurno.Cancelado;
            turno.MotivoCancelacion = "Cancelado por cliente: " + dto.Motivo;
        }
        else if (dto.Rol == "Barbero")
        {
            turno.MotivoCancelacion = "Cancelado por barbero: " + dto.Motivo;
            turno.Estado = dto.Restaurar && minutosRestantes >= 20 ? EstadoTurno.Disponible : EstadoTurno.Cancelado;
        }

        var turnoDTO = MapTurnoToDTO(turno); // 👈 Mover esto arriba de las notificaciones

        if (dto.Rol == "Cliente" && !string.IsNullOrWhiteSpace(turno.Barbero?.NotificationToken))
        {
            await _notificationsService.SendCancelacionClienteAsync(turno.Barbero.NotificationToken, turnoDTO);
        }

        if (dto.Rol == "Barbero" && !string.IsNullOrWhiteSpace(turno.Cliente?.NotificationToken))
        {
            await _notificationsService.SendCancelacionBarberoAsync(turno.Cliente.NotificationToken, turnoDTO);
        }

        await _context.SaveChangesAsync();

        return true;
    }


    public async Task EnviarNotificacionManualAsync(string token, TurnoDTO turnoDTO)
    {
        await _notificationsService.SendNotificationAsync(token, turnoDTO);
    }

    public async Task<List<HorarioTurnoDTO>> ObtenerHorariosReservadosPorBarberoYFechaAsync(int barberoId, DateTime fecha)
    {
        var fechaInicio = fecha.Date;
        var fechaFin = fechaInicio.AddDays(1);

        var turnos = await _context.Turnos
            .Where(t => t.BarberoId == barberoId &&
                        t.FechaHoraInicio >= fechaInicio &&
                        t.FechaHoraInicio < fechaFin &&
                        (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.EnProceso))
            .ToListAsync();

        return turnos.Select(t => new HorarioTurnoDTO
        {
            Inicio = t.FechaHoraInicio.ToString("HH:mm"),
            Duracion = (int)t.Duracion.TotalMinutes
        }).ToList();
    }

    public async Task<List<TurnoDTO>> ObtenerHistorialPorClienteAsync(int clienteId)
    {
        var turnos = await _context.Turnos
            .Where(t => t.ClienteId == clienteId)
            .Include(t => t.Cliente).ThenInclude(c => c.Usuario)
            .Include(t => t.Servicio)
            .Include(t => t.Barbero).ThenInclude(b => b.Usuario)
            .Include(t => t.Barbero).ThenInclude(b => b.Barberia) // ✅ Incluir barbería
            .OrderByDescending(t => t.FechaHoraInicio)
            .ToListAsync();

        return turnos.Select(t => MapTurnoToDTO(t)).ToList();
    }

    private TurnoDTO MapTurnoToDTO(Turno turno)
    {
        return new TurnoDTO
        {
            Id = turno.Id,
            BarberoId = turno.BarberoId,
            ServicioId = turno.ServicioId,
            ClienteId = turno.ClienteId,
            FechaHoraInicio = turno.FechaHoraInicio,
            HoraFin = turno.FechaHoraInicio.Add(turno.Duracion),
            Fecha = turno.FechaHoraInicio.Date,
            Duracion = turno.Duracion,
            Estado = turno.Estado,
            ClienteNombre = turno.Cliente?.Usuario?.Nombre ?? string.Empty,
            ClienteApellido = turno.Cliente?.Apellido ?? string.Empty,
            ClienteEmail = turno.Cliente?.Usuario?.Correo ?? string.Empty,
            ClienteFechaNacimiento = turno.Cliente?.FechaNacimiento ?? DateTime.MinValue,
            ServicioNombre = turno.Servicio?.Nombre ?? string.Empty,
            ServicioDescripcion = turno.Servicio?.Descripcion ?? string.Empty,
            ServicioPrecio = turno.Servicio?.Precio ?? 0,
            ServicioPrecioEspecial = turno.Servicio?.PrecioEspecial,
            MotivoCancelacion = turno.MotivoCancelacion ?? string.Empty,
            BarberoNombre = turno.Barbero?.Usuario?.Nombre ?? string.Empty,
            BarberiaNombre = turno.Barbero?.Barberia?.Usuario?.Nombre ?? string.Empty, 
           
        };
    }
}
