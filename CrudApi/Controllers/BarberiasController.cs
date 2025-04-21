using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarberiasController : ControllerBase
    {
        private readonly IBarberiaService _barberiaService;

        public BarberiasController(IBarberiaService barberiaService)
        {
            _barberiaService = barberiaService;
        }

        // GET: api/barberias
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<BarberiaResponseDTO>>> GetBarberias()
        {
            try
            {
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                var roleClaim = User.FindFirst(ClaimTypes.Role);

                if (usuarioIdClaim == null || roleClaim == null)
                    return Unauthorized("No se pudo obtener la información del token.");

                var usuarioId = int.Parse(usuarioIdClaim.Value);
                var role = roleClaim.Value;

                var roleId = role switch
                {
                    "Super_Admin" => 1,
                    "Admin" => 2,
                    "Barbero" => 2,
                    "Cliente" => 3,
                    _ => 0
                };

                var barberias = await _barberiaService.GetBarberiasAsync(usuarioId, roleId);
                return Ok(barberias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener barberías: {ex.Message}");
            }
        }

        // GET: api/barberias/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BarberiaResponseDTO?>> GetBarberia(int id)
        {
            var barberia = await _barberiaService.GetBarberiaByIdAsync(id);
            if (barberia == null) return NotFound();
            return Ok(barberia);
        }

        // POST: api/barberias
        [HttpPost]
        public async Task<IActionResult> CrearBarberia([FromBody] BarberiaCreateDTO barberiaDto)
        {
            var nuevaBarberia = await _barberiaService.CreateBarberiaAsync(barberiaDto);
            return CreatedAtAction(nameof(GetBarberia), new { id = nuevaBarberia.Id }, nuevaBarberia);
        }

        // PUT: api/barberias/5
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarBarberia(int id, [FromBody] BarberiaUpdateDTO barberiaDto)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            var barberiaActualizada = await _barberiaService.UpdateBarberiaAsync(id, barberiaDto);

            if (barberiaActualizada == null)
                return NotFound("Barbería no encontrada");

            return Ok(barberiaActualizada);
        }

        // DELETE: api/barberias/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBarberia(int id)
        {
            var deleted = await _barberiaService.DeleteBarberiaAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        // ✅ Ruta pública para el cliente (no necesita autenticación)
        [HttpGet("publicas")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerBarberiasPublicas()
        {
            var barberias = await _barberiaService.ObtenerTodasAsync();
            return Ok(barberias);
        }

        // ✅ Ruta protegida para barberías propias del usuario logueado (Admin)
        [HttpGet("mis-barberias")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ObtenerBarberiasPropias()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var barberias = await _barberiaService.ObtenerPorUsuarioId(usuarioId);
            return Ok(barberias);
        }
    }
}
