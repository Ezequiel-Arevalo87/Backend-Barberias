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
        var bloqueInicio = fechaBase.Add(dto.HoraInicio); // Ej: 2025-05-13 13:00:00
        var bloqueFin = fechaBase.Add(dto.HoraFin);       // Ej: 2025-05-13 18:00:00

        // ✅ Traer turnos que realmente estén ese día y verificar cruce
        var turnosEnConflicto = await _context.Turnos
            .Where(t =>
                t.BarberoId == dto.BarberoId &&
                (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.EnProceso) &&
                t.FechaHoraInicio.Date == fechaBase &&
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
            Console.WriteLine($"📦 dto.Fecha: {dto.Fecha}");
            Console.WriteLine($"🕓 dto.HoraInicio: {dto.HoraInicio}");
            Console.WriteLine($"🕕 dto.HoraFin: {dto.HoraFin}");
            Console.WriteLine($"📌 bloqueInicio: {bloqueInicio}");
            Console.WriteLine($"📌 bloqueFin: {bloqueFin}");
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
        Console.WriteLine("✅ Bloqueo registrado correctamente.");
        return true;
    }
}
