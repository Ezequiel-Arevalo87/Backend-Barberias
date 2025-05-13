using CrudApi.Data;
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
        // ⏰ Hora exacta en Colombia sin UTC
        var zonaColombia = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
        var fechaLocal = TimeZoneInfo.ConvertTimeFromUtc(dto.Fecha.ToUniversalTime(), zonaColombia).Date;

        var bloqueInicio = fechaLocal.Add(dto.HoraInicio);
        var bloqueFin = fechaLocal.Add(dto.HoraFin);

        var turnosEnConflicto = await _context.Turnos
            .Where(t =>
                t.BarberoId == dto.BarberoId &&
                (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.EnProceso) &&
                t.FechaHoraInicio < bloqueFin &&
                t.HoraFin > bloqueInicio
            ).ToListAsync();

        foreach (var t in turnosEnConflicto)
        {
            Console.WriteLine($"❗ Turno en conflicto: {t.Id} | {t.FechaHoraInicio} - {t.HoraFin}");
        }

        if (turnosEnConflicto.Any())
        {
            Console.WriteLine($"⛔ Se detectó cruce con turno para el barbero {dto.BarberoId} entre {bloqueInicio} y {bloqueFin}");
            throw new InvalidOperationException("No se puede bloquear este horario porque ya hay turnos asignados.");
        }

        var bloqueo = new HorarioBloqueadoBarbero
        {
            BarberoId = dto.BarberoId,
            Fecha = fechaLocal,
            HoraInicio = dto.HoraInicio,
            HoraFin = dto.HoraFin,
            Motivo = dto.Motivo
        };

        _context.HorariosBloqueados.Add(bloqueo);
        await _context.SaveChangesAsync();
        return true;
    }

}
