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
}
