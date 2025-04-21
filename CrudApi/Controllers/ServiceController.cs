using CrudApi.DTOs;
using CrudApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IBarberoService _barberoService;

        public ServiceController(IServiceService serviceService, IBarberoService barberoService)
        {
            _serviceService = serviceService;
            _barberoService = barberoService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _serviceService.GetServicesAsync());
        }

        [HttpGet("barbero/{barberoId}")]
        public async Task<IActionResult> GetServicesByBarber(int barberoId)
        {
            return Ok(await _serviceService.GetServicesByBarberAsync(barberoId));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            return service != null ? Ok(service) : NotFound();
        }

        [HttpGet("servicios-por-barbero/{barberoId}")]
        public async Task<ActionResult<List<ServiceDTO>>> GetServicePorBarbero(int barberoId)
        {
            var services = await _serviceService.GetServiciosPorBarberoAsync(barberoId);
            if (services == null || services.Count == 0)
                return NotFound($"No se encontraron servicios para el barbero con ID {barberoId}.");
            return Ok(services);
        }

        [HttpGet("mis-servicios")]
        [Authorize(Roles = "Barbero")]
        public async Task<IActionResult> ObtenerServiciosDelBarbero()
        {
            var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var barberoId = await _barberoService.ObtenerBarberoIdDesdeUsuarioIdAsync(usuarioId);
            if (barberoId == null) return NotFound("No se encontró el barbero asociado");

            var servicios = await _serviceService.GetServiciosPorBarberoAsync(barberoId.Value);
            return Ok(servicios);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ServiceDTO serviceDTO)
        {
            var createdService = await _serviceService.CreateServiceAsync(serviceDTO);
            return CreatedAtAction(nameof(GetById), new { id = createdService.Id }, createdService);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ServiceDTO serviceDTO)
        {
            var updatedService = await _serviceService.UpdateServiceAsync(id, serviceDTO);
            return updatedService != null ? Ok(updatedService) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _serviceService.DeleteServiceAsync(id) ? NoContent() : NotFound();
        }
    }
}
