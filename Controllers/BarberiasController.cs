using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<List<BarberiaDTO>>> GetBarberia()
        {
            var barberias = await _barberiaService.GetBarberiaAsync();
            return Ok(barberias);

        }

     

        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<BarberiaDTO?>> GetBarberia(int id)
        {
            var barberia = await _barberiaService.GetBarberiaByIdAsync(id);
            if (barberia == null) return NotFound();
            return Ok(barberia);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CrearBarberia([FromBody] BarberiaCreateDTO barberiaDto)
        {
            

            var nuevaBarberia = await _barberiaService.CreateBarberiaAsync(barberiaDto);
            return CreatedAtAction(nameof(GetBarberia), new { id = nuevaBarberia.Id }, nuevaBarberia);
        }

        [Authorize(Roles = "Admin,Barbero")]
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarUsuario(int id, [FromBody] BarberiaUpdateDTO barberiaDto)
        {
            if (id <= 0)
                return BadRequest("ID inválido");

            var usuarioActualizado = await _barberiaService.UpdateBarberiaAsync(id, barberiaDto);

            if (usuarioActualizado == null)
                return NotFound("Usuario no encontrado");

            return Ok(usuarioActualizado);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var deleted = await _barberiaService.DeleteBarberiaAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
