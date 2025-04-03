using CrudApi.DTOs;
using CrudApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarberoController : ControllerBase
    {
        private readonly IBarberoService _barberoService;

        public BarberoController(IBarberoService barberoService)
        {
            _barberoService = barberoService;
        }

        // Obtener todos los barberos
        [HttpGet]
        public async Task<ActionResult<List<BarberoResponseDTO>>> GetBarbero()
        {
            var barberos = await _barberoService.GetBarberoAsync();
            return Ok(barberos);
        }

        // Obtener barbero por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<BarberoResponseDTO>> GetBarbero(int id)
        {
            var barbero = await _barberoService.GetBarberoByIdAsync(id);
            if (barbero == null)
                return NotFound($"No se encontró un barbero con el ID {id}.");
            return Ok(barbero);
        }

        // Obtener barberos por barbería
        [HttpGet("barberia/{barberiaId}")]
        public async Task<ActionResult<List<BarberoResponseDTO>>> GetBarberosPorBarberia(int barberiaId)
        {
            var barberos = await _barberoService.GetBarberosPorBarberiaAsync(barberiaId);

            if (barberos == null || barberos.Count == 0)
                return NotFound($"No se encontraron barberos para la barbería con ID {barberiaId}.");

            return Ok(barberos);
        }

        // Crear nuevo barbero
        [HttpPost]
        public async Task<ActionResult<BarberoResponseDTO>> CrearBarbero([FromBody] BarberoCreateDTO barberoDto)
        {
            try
            {
                var nuevoBarbero = await _barberoService.CreateBarberoAsync(barberoDto);
                return CreatedAtAction(nameof(GetBarbero), new { id = nuevoBarbero.Id }, nuevoBarbero);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}
