using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class TurnoController : ControllerBase
{
    private readonly ITurnoService _turnoService;
    private readonly ApplicationDbContext _context;


    public TurnoController(ITurnoService turnoService, ApplicationDbContext context)
    {
        _turnoService = turnoService;
        _context = context; // ✅ listo

    }

    // Obtener todos los turnos o filtrados por barbero
    [HttpGet]
    public async Task<IActionResult> ObtenerTurnos([FromQuery] int? barberoId)
    {
        var turnos = await _turnoService.ObtenerTurnosAsync(barberoId);
        return Ok(turnos);
    }

    // Crear turno
    [HttpPost]
    public async Task<IActionResult> CrearTurno([FromBody] TurnoCreateDTO turnoCreateDTO)
    {
        try
        {
            var turnoCreado = await _turnoService.CrearTurnoAsync(turnoCreateDTO);
            await _turnoService.NotificarTurnoAsync(turnoCreado);

            return CreatedAtAction(nameof(ObtenerTurnos), new { turnoId = turnoCreado.Id }, turnoCreado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al crear el turno: {ex.Message}");
        }
    }

    // Obtener horarios reservados por fecha (lo que usa la app móvil)
    [HttpGet("barbero/{barberoId}")]
    public async Task<IActionResult> ObtenerTurnosPorBarberoYFecha(int barberoId, [FromQuery] DateTime fecha)
    {
        var horarios = await _turnoService.ObtenerHorariosReservadosPorBarberoYFechaAsync(barberoId, fecha);
        return Ok(horarios);
    }

    // Notificación manual (usado en pruebas o admin)
    [HttpPost("notificar")]
    public async Task<IActionResult> NotificarTurno(string token, [FromBody] TurnoDTO turnoDTO, [FromQuery] bool paraCliente = false)
    {
        try
        {
            await _turnoService.EnviarNotificacionManualAsync(token, turnoDTO, paraCliente);
            return Ok(new { mensaje = "Notificación enviada" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al enviar la notificación: {ex.Message}");
        }
    }


    // Cancelar turno
    [HttpPut("cancelar")]
    public async Task<IActionResult> CancelarTurno([FromBody] CancelarTurnoDTO dto)
    {
        var result = await _turnoService.CancelarTurnoAsync(dto);

        if (!result)
            return NotFound("Turno no encontrado o no se pudo cancelar");

        return Ok("Turno cancelado correctamente");
    }

    [HttpGet("historial-cliente/{clienteId}")]
    public async Task<IActionResult> ObtenerHistorialCliente(int clienteId)
    {
        var historial = await _turnoService.ObtenerHistorialPorClienteAsync(clienteId);
        return Ok(historial);
    }
    [HttpPost("barbero/reporte")]
    [Authorize(Roles = "Barbero")]
    public async Task<IActionResult> ObtenerTurnosBarbero([FromBody] FiltroReporteTurnoDTO filtro)
    {
        var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(usuarioIdClaim) || !int.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized("Usuario no autenticado o ID inválido");

        var barbero = await _context.Barberos
            .Include(b => b.Usuario)
            .FirstOrDefaultAsync(b => b.UsuarioId == usuarioId);

        if (barbero == null || barbero.Usuario == null)
            return Unauthorized("No se encontró el barbero autenticado o su usuario asociado");

        try
        {
            var turnos = await _turnoService.ObtenerTurnosDelBarberoAsync(barbero.Id, filtro);
            return Ok(turnos);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error interno al consultar los turnos del barbero: {ex.Message}");
            return StatusCode(500, "Error interno al generar el reporte del barbero.");
        }
    }


}
