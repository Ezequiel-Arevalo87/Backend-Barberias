using CrudApi.Data;
using CrudApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

public class HorarioService : IHorarioService
{
    private readonly ApplicationDbContext _context;

    public HorarioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<HorarioBarberiaDTO>> GetHorariosAsync(int barberiaId)
    {
        return await _context.HorariosBarberia
            .Where(h => h.BarberiaId == barberiaId)
            .Select(h => new HorarioBarberiaDTO
            {
                Id = h.Id,
                BarberiaId = h.BarberiaId,
                DiaSemana = h.DiaSemana,
                Abierto = h.Abierto ?? false,
                HoraInicio = h.HoraInicio.ToString(@"hh\:mm"), // Convertir TimeSpan a string
                HoraFin = h.HoraFin.ToString(@"hh\:mm")
            })
            .ToListAsync();
    }

    public async Task<HorarioBarberiaDTO?> GetHorarioByIdAsync(int id)
    {
        var horario = await _context.HorariosBarberia.FindAsync(id);
        if (horario == null) return null;

        return new HorarioBarberiaDTO
        {
            Id = horario.Id,
            BarberiaId = horario.BarberiaId,
            DiaSemana = horario.DiaSemana,
            Abierto = horario.Abierto ?? false, // o true según tu lógica

            HoraInicio = horario.HoraInicio.ToString(@"hh\:mm"),
            HoraFin = horario.HoraFin.ToString(@"hh\:mm")
        };
    }

    public async Task<HorarioBarberiaDTO> CrearHorarioAsync(HorarioBarberiaDTO horarioBarberiaDTO)
    {
        // Convertir string a TimeSpan
        if (!TimeSpan.TryParse(horarioBarberiaDTO.HoraInicio, out TimeSpan horaInicio) ||
            !TimeSpan.TryParse(horarioBarberiaDTO.HoraFin, out TimeSpan horaFin))
        {
            throw new ArgumentException("HoraInicio o HoraFin tienen un formato incorrecto. Use HH:mm");
        }

        // Verificar si ya existe un horario para el mismo día y barbería
        var horarioExistente = await _context.HorariosBarberia
            .FirstOrDefaultAsync(h => h.BarberiaId == horarioBarberiaDTO.BarberiaId && h.DiaSemana == horarioBarberiaDTO.DiaSemana);

        if (horarioExistente != null)
        {
            // Actualizar horario existente
            horarioExistente.HoraInicio = horaInicio;
            horarioExistente.HoraFin = horaFin;
            horarioExistente.EsFestivo = horarioBarberiaDTO.EsFestivo;
            horarioExistente.Abierto = horarioBarberiaDTO.Abierto;

            _context.HorariosBarberia.Update(horarioExistente);
            await _context.SaveChangesAsync();

            horarioBarberiaDTO.Id = horarioExistente.Id;
        }
        else
        {
            // Crear nuevo horario si no existe
            var nuevoHorario = new HorarioBarberia
            {
                BarberiaId = horarioBarberiaDTO.BarberiaId,
                DiaSemana = horarioBarberiaDTO.DiaSemana,
                HoraInicio = horaInicio,
                HoraFin = horaFin,
                EsFestivo = horarioBarberiaDTO.EsFestivo,
                Abierto = horarioBarberiaDTO.Abierto
            };

            _context.HorariosBarberia.Add(nuevoHorario);
            await _context.SaveChangesAsync();

            horarioBarberiaDTO.Id = nuevoHorario.Id;
        }

        return horarioBarberiaDTO;
    }

    public async Task<bool> ActualizarHorarioAsync(HorarioBarberiaDTO horarioBarberiaDTO)
    {
        var horario = await _context.HorariosBarberia.FindAsync(horarioBarberiaDTO.Id);
        if (horario == null) return false;

        if (!TimeSpan.TryParse(horarioBarberiaDTO.HoraInicio, out TimeSpan horaInicio) ||
            !TimeSpan.TryParse(horarioBarberiaDTO.HoraFin, out TimeSpan horaFin))
        {
            throw new ArgumentException("HoraInicio o HoraFin tienen un formato incorrecto. Use HH:mm");
        }

        horario.HoraInicio = horaInicio;
        horario.HoraFin = horaFin;

        _context.Entry(horario).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarHorarioAsync(int id)
    {
        var horario = await _context.HorariosBarberia.FindAsync(id);
        if (horario == null) return false;

        _context.HorariosBarberia.Remove(horario);
        await _context.SaveChangesAsync();
        return true;
    }
}
