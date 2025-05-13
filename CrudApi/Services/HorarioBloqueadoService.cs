using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Models;
using Microsoft.EntityFrameworkCore;

public class HorarioBloqueadoService : IHorarioBloqueadoService
{
    private readonly ApplicationDbContext _context;

    public HorarioBloqueadoService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CrearBloqueoAsync(CrearHorarioBloqueadoDTO dto)
    {
        var zonaColombia = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

        var fechaBase = dto.Fecha.Date;
        var bloqueInicio = fechaBase.Add(dto.HoraInicio); // hora local
        var bloqueFin = fechaBase.Add(dto.HoraFin);       // hora local

        // Obtener turnos del barbero para ese día en UTC
        var turnos = await _context.Turnos
            .Where(t => t.BarberoId == dto.BarberoId &&
                        (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.EnProceso))
            .ToListAsync();

        // Convertir los turnos a hora local para comparar correctamente
        var turnosEnConflicto = turnos.Where(t =>
        {
            var inicioCol = TimeZoneInfo.ConvertTimeFromUtc(t.FechaHoraInicio, zonaColombia);
            var finCol = TimeZoneInfo.ConvertTimeFromUtc(t.HoraFin, zonaColombia);

            Console.WriteLine($"❗ Turno en conflicto: {t.Id} | {inicioCol} - {finCol}");

            return inicioCol < bloqueFin && finCol > bloqueInicio;
        }).ToList();

        if (turnosEnConflicto.Any())
        {
           

            throw new InvalidOperationException("No se puede bloquear este horario porque ya hay turnos asignados.");
        }

        var bloqueo = new HorarioBloqueadoBarbero
        {
            BarberoId = dto.BarberoId,
            Fecha = fechaBase,
            HoraInicio = dto.HoraInicio,
            HoraFin = dto.HoraFin,
            Motivo = dto.Motivo
        };

        _context.HorariosBloqueados.Add(bloqueo);
        await _context.SaveChangesAsync();
        return true;
    }
    public async Task<List<HorarioTurnoDTO>> ObtenerBloqueosAsync(int barberoId, DateTime fecha)
    {
        var zonaColombia = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");

        var bloqueos = await _context.HorariosBloqueados
            .Where(h => h.BarberoId == barberoId && h.Fecha.Date == fecha.Date)
            .ToListAsync();

        var resultado = bloqueos.Select(b =>
        {
            var horaInicioLocal = b.Fecha.Date.Add(b.HoraInicio); // No se convierte a UTC
            var duracion = (int)(b.HoraFin - b.HoraInicio).TotalMinutes;

            return new HorarioTurnoDTO
            {
                Inicio = horaInicioLocal.ToString("HH:mm"),
                Duracion = duracion
            };
        }).ToList();

        return resultado;
    }


}
