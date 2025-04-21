using CrudApi.Data;
using CrudApi.Models;
using CrudApi.Notifications;
using CrudApi.DTOs;
using Microsoft.EntityFrameworkCore;
using CrudApi.Interfaces;

public class ShiftService : IShiftService
{
    private readonly ApplicationDbContext _context;
    private readonly Notifications _notifications;

    public ShiftService(ApplicationDbContext context, Notifications notifications)
    {
        _context = context;
        _notifications = notifications;
    }

    public async Task CerrarTurnosVencidosAsync()
    {
        var now = DateTime.Now;

        var turnos = await _context.Turnos
            .Include(t => t.Cliente).ThenInclude(c => c.Usuario)
            .Include(t => t.Barbero).ThenInclude(b => b.Usuario)
            .Include(t => t.Servicio)
            .Where(t => t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.EnProceso)
            .ToListAsync();

        foreach (var turno in turnos)
        {
            var horaFin = turno.FechaHoraInicio.Add(turno.Duracion);
            EstadoTurno? nuevoEstado = null;

            if (turno.Estado == EstadoTurno.Pendiente && now >= turno.FechaHoraInicio && now < horaFin)
            {
                nuevoEstado = EstadoTurno.EnProceso;
            }
            else if ((turno.Estado == EstadoTurno.Pendiente || turno.Estado == EstadoTurno.EnProceso) && now >= horaFin)
            {
                nuevoEstado = EstadoTurno.Cerrado;
            }

            if (nuevoEstado != null)
            {
                turno.Estado = nuevoEstado.Value;
                turno.HoraFin = horaFin; // ✅ Guardamos la hora de finalización real

                // ✅ Notificar al barbero
                var tokenBarbero = turno.Barbero?.NotificationToken;
                if (!string.IsNullOrWhiteSpace(tokenBarbero))
                {
                    var turnoDTO = new TurnoDTO
                    {
                        Id = turno.Id,
                        BarberoId = turno.BarberoId,
                        ClienteId = turno.ClienteId,
                        ServicioId = turno.ServicioId,
                        FechaHoraInicio = turno.FechaHoraInicio,
                        HoraFin = horaFin,
                        Fecha = turno.FechaHoraInicio.Date,
                        Duracion = turno.Duracion,
                        Estado = turno.Estado,
                        ClienteNombre = turno.Cliente?.Usuario?.Nombre ?? "",
                        ClienteApellido = turno.Cliente?.Apellido ?? "",
                        ServicioNombre = turno.Servicio?.Nombre ?? "",
                        ServicioDescripcion = turno.Servicio?.Descripcion ?? "",
                        ServicioPrecio = turno.Servicio?.Precio ?? 0,
                        ServicioPrecioEspecial = turno.Servicio?.PrecioEspecial
                    };

                    await _notifications.SendNotificationAsync(
                        tokenBarbero,
                        $"Turno {nuevoEstado}",
                        $"El turno con {turnoDTO.ClienteNombre} está ahora en estado: {nuevoEstado}",
                        turnoDTO
                    );
                }
            }
        }

        await _context.SaveChangesAsync();
    }
}
