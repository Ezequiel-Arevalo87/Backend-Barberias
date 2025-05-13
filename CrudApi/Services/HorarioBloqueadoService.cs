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
        var fechaBase = dto.Fecha.Date;

        // 🔒 Estas horas se interpretan como Local, sin UTC
        var bloqueInicio = DateTime.SpecifyKind(fechaBase.Add(dto.HoraInicio), DateTimeKind.Local);
        var bloqueFin = DateTime.SpecifyKind(fechaBase.Add(dto.HoraFin), DateTimeKind.Local);

        Console.WriteLine($"📦 dto.Fecha: {dto.Fecha}");
        Console.WriteLine($"🕓 dto.HoraInicio: {dto.HoraInicio}");
        Console.WriteLine($"🕕 dto.HoraFin: {dto.HoraFin}");
        Console.WriteLine($"📌 bloqueInicio: {bloqueInicio} ({bloqueInicio.Kind})");
        Console.WriteLine($"📌 bloqueFin: {bloqueFin} ({bloqueFin.Kind})");

        var turnos = await _context.Turnos
            .Where(t =>
                t.BarberoId == dto.BarberoId &&
                (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.EnProceso) &&
                t.FechaHoraInicio < bloqueFin &&
                t.HoraFin > bloqueInicio
            )
            .ToListAsync();

        if (turnos.Any())
        {
            foreach (var t in turnos)
            {
                Console.WriteLine($"❗ Turno en conflicto: {t.Id} | {t.FechaHoraInicio} - {t.HoraFin}");
            }

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

}
