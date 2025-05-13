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
        

        var bloqueInicio = DateTime.SpecifyKind(fechaBase.Add(dto.HoraInicio), DateTimeKind.Utc);
        var bloqueFin = DateTime.SpecifyKind(fechaBase.Add(dto.HoraFin), DateTimeKind.Utc);


        var tieneTurnos = await _context.Turnos.AnyAsync(t =>
            t.BarberoId == dto.BarberoId &&
            (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.EnProceso) &&
            t.FechaHoraInicio < bloqueFin &&
            t.HoraFin > bloqueInicio
        );

        if (tieneTurnos)
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
        return true;
    }
}

