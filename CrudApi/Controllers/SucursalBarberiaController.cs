using CrudApi.DTOs;
using CrudApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SucursalBarberiaController : ControllerBase
    {
        private readonly ISucursalBarberiaService _service;

        public SucursalBarberiaController(ISucursalBarberiaService service)
        {
            _service = service;
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<SucursalBarberiaDTO>>> GetAll()
        {
            var sucursales = await _service.GetAllAsync();
            return Ok(sucursales);
        }

        // [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<SucursalBarberiaDTO>> GetById(int id)
        {
            var sucursal = await _service.GetByIdAsync(id);
            if (sucursal == null)
                return NotFound();

            return Ok(sucursal);
        }

        // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<SucursalBarberiaDTO>> Create([FromBody] SucursalBarberiaDTO dto)
        {
            var nueva = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = nueva.Id }, nueva);
        }

        // [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] SucursalBarberiaDTO dto)
        {
            var actualizada = await _service.UpdateAsync(id, dto);
            if (actualizada == null)
                return NotFound();

            return Ok(actualizada);
        }

        // [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
