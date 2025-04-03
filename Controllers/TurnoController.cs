using CrudApi.DTOs;
using CrudApi.Notifications;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TurnoController : ControllerBase
{
    private readonly ITurnoService _turnoService;

    public TurnoController(ITurnoService turnoService)
    {
        _turnoService = turnoService;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTurnos([FromQuery] int? barberoId)
    {
        var turnos = await _turnoService.ObtenerTurnosAsync(barberoId);
        return Ok(turnos);
    }

    [HttpPost]
    public async Task<IActionResult> CrearTurno([FromBody] TurnoCreateDTO turnoCreateDTO)
    {
        try
        {
            // Crear el turno y obtener el DTO completo
            var turnoCreado = await _turnoService.CrearTurnoAsync(turnoCreateDTO);

            // Enviar notificación solo si se crea correctamente
            await _turnoService.NotificarTurnoAsync(turnoCreado);

            return CreatedAtAction(nameof(ObtenerTurnos), new { turnoId = turnoCreado.Id }, turnoCreado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al crear el turno: {ex.Message}");
        }
    }

    [HttpPost("notificar")]
    public async Task<IActionResult> NotificarTurno(string token, [FromBody] TurnoDTO turnoDTO)
    {
        try
        {
            await _turnoService.EnviarNotificacionManualAsync(token, turnoDTO);
            return Ok(new { mensaje = "Notificación enviada" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al enviar la notificación: {ex.Message}");
        }
    }

    //[HttpPut("{turnoId}")]
    //public async Task<IActionResult> ActualizarEstadoTurno(int turnoId, [FromBody] string nuevoEstado)
    //{
    //    try
    //    {
    //        await _turnoService.ActualizarEstadoTurnoAsync(turnoId, nuevoEstado);
    //        return NoContent();
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Error al actualizar el turno: {ex.Message}");
    //    }
    //}

    //[HttpPost("cerrar-turnos-vencidos")]
    //public async Task<IActionResult> CerrarTurnosVencidos()
    //{
    //    try
    //    {
    //        await _turnoService.CerrarTurnosVencidosAsync();
    //        return NoContent();
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Error al cerrar turnos vencidos: {ex.Message}");
    //    }
    //}
}
