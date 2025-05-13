using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class HorarioBloqueadoController : ControllerBase
{
    private readonly IHorarioBloqueadoService _service;

    public HorarioBloqueadoController(IHorarioBloqueadoService service)
    {
        _service = service;
    }

    [HttpPost("bloquear")]
    public async Task<IActionResult> BloquearHorario([FromBody] CrearHorarioBloqueadoDTO dto)
    {
        try
        {
            var resultado = await _service.CrearBloqueoAsync(dto);
            return Ok(new { exito = resultado, mensaje = "Horario bloqueado correctamente." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { exito = false, mensaje = ex.Message });
        }
    }

    [HttpGet("barbero/{barberoId}")]
    public async Task<IActionResult> ObtenerBloqueos(int barberoId, [FromQuery] DateTime fecha)
    {
        try
        {
            var bloqueos = await _service.ObtenerBloqueosAsync(barberoId, fecha);
            return Ok(bloqueos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error al obtener bloqueos: {ex.Message}");
            return BadRequest("No se pudieron obtener los horarios bloqueados.");
        }
    }


}

