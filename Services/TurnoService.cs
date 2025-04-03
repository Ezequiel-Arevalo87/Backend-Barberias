using CrudApi.DTOs;
using CrudApi.Notifications;
using CrudApi.Data;
using Microsoft.EntityFrameworkCore;
using CrudApi.Models;

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
            .Include(t => t.Cliente)
            .Include(t => t.Servicio)
            .AsQueryable();

        if (barberoId.HasValue)
        {
            query = query.Where(t => t.BarberoId == barberoId.Value);
        }

        var turnos = await query.ToListAsync();



        var dtoList = turnos.Select(turno => new TurnoDTO
        {
            Id = turno.TurnoId,
            BarberoId = turno.BarberoId,
            ServicioId = turno.ServicioId,
            ClienteId = turno.ClienteId,
            FechaHoraInicio = turno.FechaHoraInicio,
        
            Fecha = turno.FechaHoraInicio.Date,
            Duracion = turno.Duracion,
            Estado = turno.Estado,

            ClienteNombre = turno.Cliente?.Nombre ?? string.Empty,
            ClienteApellido = turno.Cliente?.Apellido ?? string.Empty,
            ClienteEmail = turno.Cliente?.Email ?? string.Empty,
            ClienteFechaNacimiento = turno.Cliente?.FechaNacimiento ?? DateTime.MinValue,

            ServicioNombre = turno.Servicio?.Servicio ?? string.Empty,
            ServicioDescripcion = turno.Servicio?.Descripcion ?? string.Empty,
            ServicioPrecio = turno.Servicio?.Precio ?? 0,
            ServicioPrecioEspecial = turno.Servicio?.PrecioEspecial
        }).ToList();

        return dtoList;
    }

    public async Task<TurnoDTO> CrearTurnoAsync(TurnoCreateDTO turnoCreateDTO)
    {
        var turno = new Turno
        {
            FechaHora = turnoCreateDTO.FechaHoraInicio, // si lo necesitas
            BarberoId = turnoCreateDTO.BarberoId,
            ClienteId = turnoCreateDTO.ClienteId,
            ServicioId = turnoCreateDTO.ServicioId,
          
            FechaHoraInicio = turnoCreateDTO.FechaHoraInicio,
            Duracion = TimeSpan.FromMinutes(turnoCreateDTO.Duracion),
            Estado = turnoCreateDTO.Estado
        };

        _context.Turnos.Add(turno);
        await _context.SaveChangesAsync();

        return new TurnoDTO
        {
            Id = turno.TurnoId,
            BarberoId = turno.BarberoId,
            ClienteId = turno.ClienteId,
            ServicioId = turno.ServicioId,
            FechaHoraInicio = turno.FechaHoraInicio,
            HoraFin = turno.FechaHoraInicio.Add(turno.Duracion),
            Fecha = turno.FechaHoraInicio.Date,
            Duracion = turno.Duracion,
            Estado = turno.Estado
        };
    }


    public async Task<TurnoDTO> NotificarTurnoAsync(TurnoDTO turnoInput)
    {
        // Buscar el turno real con datos completos
        var turno = await _context.Turnos
            .Include(t => t.Cliente)
            .Include(t => t.Servicio)
            .FirstOrDefaultAsync(t => t.TurnoId == turnoInput.Id);

        if (turno == null)
            throw new Exception("❌ No se encontró el turno.");

        // Obtener token real del cliente
        var token = "cVu0ZB52FKsd04dZo2f0tM:APA91bFF3COWQrhaGL4qkaeEWkdeEGIeievh2yhm4Ahx0y_vFtkZ-FyPq90klcsr-_EcnnEd4ehKhvcfqVt6c0aHvRPVh8po5pK4hPE4M8W2JwQFxMIGGSM";

        if (string.IsNullOrWhiteSpace(token))
            throw new Exception("❌ El cliente no tiene un token FCM válido registrado.");

        // Construir DTO para notificación (opcional si quieres retornarlo al frontend)
        var turnoDTO = new TurnoDTO
        {
            Id = turno.TurnoId,
            BarberoId = turno.BarberoId,
            ServicioId = turno.ServicioId,
            ClienteId = turno.ClienteId,
            FechaHoraInicio = turno.FechaHoraInicio,
            Fecha = turno.FechaHoraInicio.Date,
            Duracion = turno.Duracion,
            Estado = turno.Estado,
            ClienteNombre = turno.Cliente?.Nombre ?? string.Empty,
            ClienteApellido = turno.Cliente?.Apellido ?? string.Empty,
            ClienteEmail = turno.Cliente?.Email ?? string.Empty,
            ClienteFechaNacimiento = turno.Cliente?.FechaNacimiento ?? DateTime.MinValue,
            ServicioNombre = turno.Servicio?.Servicio ?? string.Empty,
            ServicioDescripcion = turno.Servicio?.Descripcion ?? string.Empty,
            ServicioPrecio = turno.Servicio?.Precio ?? 0,
            ServicioPrecioEspecial = turno.Servicio?.PrecioEspecial
        };

       

        // Enviar notificación
        await _notificationsService.SendNotificationAsync(
            token,
            "Nuevo Turno",
            $"Tienes un nuevo turno con {turnoDTO.ClienteNombre} {turnoDTO.ClienteApellido} el {turnoDTO.FechaHoraInicio}",
            turnoDTO
        );

        return turnoDTO;
    }

    public async Task EnviarNotificacionManualAsync(string token, TurnoDTO turnoDTO)
    {
        await _notificationsService.SendNotificationAsync(
            token,
            "Notificación Manual",
            "Tienes un nuevo turno asignado manualmente",
            turnoDTO
        );
    }

  
}
