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
            await _notificationsService.SendNotificationAsync(turno.Cliente.NotificationToken, turnoDTO, true); // ✅ para el cliente

        if (!string.IsNullOrWhiteSpace(turno.Barbero?.NotificationToken))
            await _notificationsService.SendNotificationAsync(turno.Barbero.NotificationToken, turnoDTO, false); // ✅ para el barbero


        turno.Notificado = true;
        await _context.SaveChangesAsync();

        return turnoDTO;
    }

    public async Task<bool> CancelarTurnoAsync(CancelarTurnoDTO dto)
    {
        // Buscar el turno incluyendo las relaciones necesarias
        var turno = await _context.Turnos
            .Include(t => t.Cliente).ThenInclude(c => c.Usuario)
            .Include(t => t.Barbero).ThenInclude(b => b.Usuario)
            .Include(t => t.Servicio)
            .FirstOrDefaultAsync(t => t.Id == dto.TurnoId);

        if (turno == null)
        {
            Console.WriteLine("❌ No se encontró el turno.");
            return false;
        }

        // Calcular minutos restantes para el inicio del turno
        var minutosRestantes = (turno.FechaHoraInicio - DateTime.UtcNow).TotalMinutes;
        Console.WriteLine($"🕒 Minutos restantes para el turno: {minutosRestantes}");

        if (dto.Rol == "Cliente")
        {
            // ✅ Cancelación por parte del cliente (automática según minutos)
            if (minutosRestantes >= 20)
            {
                turno.Estado = EstadoTurno.Disponible;
                Console.WriteLine("✅ Turno marcado como DISPONIBLE (cancelado por cliente con anticipación).");
            }
            else
            {
                turno.Estado = EstadoTurno.Cancelado;
                Console.WriteLine("✅ Turno marcado como CANCELADO (cancelado por cliente sin anticipación).");
            }

            turno.MotivoCancelacion = $"Cancelado por cliente: {dto.Motivo}";
        }
        else if (dto.Rol == "Barbero")
        {
            // ✅ Cancelación por parte del barbero (elige restaurar o no)
            turno.MotivoCancelacion = $"Cancelado por barbero: {dto.Motivo}";

            if (dto.Restaurar && minutosRestantes >= 20)
            {
                turno.Estado = EstadoTurno.Disponible;
                Console.WriteLine("✅ Turno marcado como DISPONIBLE (cancelado por barbero y habilitado).");
            }
            else
            {
                turno.Estado = EstadoTurno.Cancelado;
                Console.WriteLine("✅ Turno marcado como CANCELADO (cancelado por barbero definitivamente).");
            }
        }
        else
        {
            Console.WriteLine("❌ Rol inválido recibido.");
            return false;
        }

        await _context.SaveChangesAsync();

        // Mapear el turno actualizado para enviar en notificación
        var turnoDTO = MapTurnoToDTO(turno);

        // 🔔 Enviar notificaciones
        if (dto.Rol == "Cliente")
        {
            if (!string.IsNullOrWhiteSpace(turno.Cliente?.NotificationToken))
            {
                await _notificationsService.EnviarNotificacionCancelacionClienteAsync(
                    turno.Cliente.NotificationToken, turnoDTO, dto.Motivo);
            }

            if (!string.IsNullOrWhiteSpace(turno.Barbero?.NotificationToken))
            {
                await _notificationsService.EnviarNotificacionCancelacionBarberoAsync(
                    turno.Barbero.NotificationToken, turnoDTO, dto.Motivo);
            }
        }
        else if (dto.Rol == "Barbero")
        {
            if (!string.IsNullOrWhiteSpace(turno.Cliente?.NotificationToken))
            {
                await _notificationsService.EnviarNotificacionCancelacionPorBarberoAsync(
                    turno.Cliente.NotificationToken, turnoDTO, dto.Motivo);
            }

            if (!string.IsNullOrWhiteSpace(turno.Barbero?.NotificationToken))
            {
                await _notificationsService.EnviarNotificacionCancelacionClienteAsync(
                    turno.Barbero.NotificationToken, turnoDTO, dto.Motivo);
            }
        }

        Console.WriteLine("✅ Cancelación procesada correctamente.");
        return true;
    }


    public async Task EnviarNotificacionManualAsync(string token, TurnoDTO turnoDTO, bool paraCliente)
    {
        await _notificationsService.SendNotificationAsync(token, turnoDTO, paraCliente);
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
            .OrderByDescending(t => t.FechaHoraInicio)
            .ToListAsync();

        return turnos.Select(t => MapTurnoToDTO(t)).ToList();
    }

    private TurnoDTO MapTurnoToDTO(Turno turno)
    {
        var fechaLocal = DateTime.SpecifyKind(turno.FechaHoraInicio, DateTimeKind.Local); // 👈 Aquí te aseguras de que siempre se trate como local

        return new TurnoDTO
        {
            Id = turno.Id,
            BarberoId = turno.BarberoId,
            ServicioId = turno.ServicioId,
            ClienteId = turno.ClienteId,
            FechaHoraInicio = fechaLocal,
            HoraFin = fechaLocal.Add(turno.Duracion),
            Fecha = fechaLocal.Date,
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
            BarberoNombre = turno.Barbero?.Usuario.Nombre ?? "",
            MotivoCancelacion = turno.MotivoCancelacion,
        };
    }
    public async Task<List<TurnoDTO>> ObtenerTurnosDelBarberoAsync(int barberoId, FiltroReporteTurnoDTO filtro)
    {
        var query = _context.Turnos
            .Include(t => t.Cliente)
                .ThenInclude(c => c.Usuario)
            .Include(t => t.Servicio)
            .Include(t => t.Barbero)
                .ThenInclude(b => b.Usuario)
            .Include(t => t.Barbero)
                .ThenInclude(b => b.Barberia)
                    .ThenInclude(barb => barb.Usuario)
            .Where(t => t.BarberoId == barberoId)
            .AsQueryable();

        if (filtro.FechaInicio.HasValue)
            query = query.Where(t => t.FechaHoraInicio >= filtro.FechaInicio.Value);

        if (filtro.FechaFin.HasValue)
            query = query.Where(t => t.FechaHoraInicio <= filtro.FechaFin.Value);

        var turnos = await query.ToListAsync();

        // 🔍 Filtrar turnos con datos incompletos
        turnos = turnos.Where(t =>
       t.Cliente?.Usuario != null &&
       t.Barbero?.Usuario != null &&
       t.Barbero?.Barberia?.Usuario != null &&
       t.Servicio != null
   ).ToList();


        // 🧪 Log (opcional para debugging)
        foreach (var t in turnos)
        {
            Console.WriteLine($"✔ TurnoID: {t.Id} - Cliente: {t.Cliente.Usuario.Nombre}, Barbero: {t.Barbero.Usuario.Nombre}, Servicio: {t.Servicio.Nombre}");
        }

        // 🧾 Proyección segura
        var resultado = turnos.Select(t => new TurnoDTO
        {
            Id = t.Id,
            BarberoId = t.BarberoId,
            ClienteId = t.ClienteId,
            ServicioId = t.ServicioId,
            FechaHoraInicio = t.FechaHoraInicio,
            HoraFin = t.FechaHoraInicio.Add(t.Duracion),
            Duracion = t.Duracion,
            Estado = t.Estado,

            ClienteNombre = t.Cliente.Usuario.Nombre,
            ClienteApellido = t.Cliente.Apellido,
            ClienteEmail = t.Cliente.Usuario.Correo,
            ClienteFechaNacimiento = t.Cliente.FechaNacimiento,

            ServicioNombre = t.Servicio.Nombre,
            ServicioDescripcion = t.Servicio.Descripcion,
            ServicioPrecio = t.Servicio.Precio,
            ServicioPrecioEspecial = t.Servicio.PrecioEspecial,

            BarberoNombre = t.Barbero.Usuario.Nombre,
            BarberiaNombre = t.Barbero.Barberia.Usuario.Nombre,

            MotivoCancelacion = t.MotivoCancelacion
        }).ToList();

        return resultado;
    }

}
