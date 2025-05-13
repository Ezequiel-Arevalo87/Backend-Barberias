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
        // Validar si hay turnos existentes en el rango a bloquear
        var tieneTurnos = await _context.Turnos.AnyAsync(t =>
      t.BarberoId == dto.BarberoId &&
      t.FechaHoraInicio.Date == dto.Fecha.Date &&
      t.FechaHoraInicio.TimeOfDay < dto.HoraFin &&
      t.HoraFin.TimeOfDay > dto.HoraInicio &&
      (t.Estado == EstadoTurno.Pendiente || t.Estado == EstadoTurno.EnProceso)
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
