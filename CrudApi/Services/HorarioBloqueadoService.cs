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
        var bloqueInicio = dto.Fecha.Date + dto.HoraInicio;
        var bloqueFin = dto.Fecha.Date + dto.HoraFin;

        var tieneTurnos = await _context.Turnos.AnyAsync(t =>
            t.BarberoId == dto.BarberoId &&
            (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.EnProceso) &&
            t.FechaHoraInicio < bloqueFin &&
            t.HoraFin > bloqueInicio
        );

        if (tieneTurnos)
            throw new InvalidOperationException("No se puede bloquear este horario porque ya hay turnos asignados.");

        var bloqueo = new HorarioBloqueadoBarbero
        {
            BarberoId = dto.BarberoId,
            Fecha = dto.Fecha,
            HoraInicio = dto.HoraInicio,
            HoraFin = dto.HoraFin,
            Motivo = dto.Motivo
        };

        _context.HorariosBloqueados.Add(bloqueo);
        await _context.SaveChangesAsync();
        return true;
    }
}

