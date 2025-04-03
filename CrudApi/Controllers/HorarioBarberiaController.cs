using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class HorarioController : ControllerBase
{
    private readonly IHorarioService _horarioService;

    public HorarioController(IHorarioService horarioService)
    {
        _horarioService = horarioService;
    }

    [HttpGet("{barberiaId}")]
    public async Task<IActionResult> GetHorarios(int barberiaId)
    {
        var horarios = await _horarioService.GetHorariosAsync(barberiaId);
        return Ok(horarios);
    }

    [HttpGet("detalle/{id}")]
    public async Task<IActionResult> GetHorarioById(int id)
    {
        var horario = await _horarioService.GetHorarioByIdAsync(id);
        if (horario == null) return NotFound();
        return Ok(horario);
    }

    [HttpPost]
    public async Task<IActionResult> CrearHorarios([FromBody] List<HorarioBarberiaDTO> horariosBarberiaDTO)
    {
        foreach (var horario in horariosBarberiaDTO)
        {
            // Validación solo si está cerrado
            if (horario.DiaSemana == "Domingo" && !horario.Abierto)
            {
                horario.EsFestivo = true;
                horario.HoraInicio = "00:00";
                horario.HoraFin = "00:00";
            }

            // Si está abierto, respeta el horario enviado
            await _horarioService.CrearHorarioAsync(horario);
        }

        return Ok("Horarios creados o actualizados exitosamente");
    }


}
