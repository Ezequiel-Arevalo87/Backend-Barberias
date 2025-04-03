using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CrudApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }



        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _serviceService.GetServicesAsync());
        }

        [HttpGet("servicio/barbero/{barberoId}")]
        public async Task<IActionResult> GetServicesByBarber(int barberoId)
        {
            return Ok(await _serviceService.GetServicesByBarberAsync(barberoId));
        }

        [HttpGet("turnos/barbero/{barberoId}")]
        public async Task<IActionResult> GetTurnosPorBarbero(int barberoId)
        {
            var turnos = await _serviceService.GetTurnosPorBarberoAsync(barberoId);
            return Ok(turnos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var service = await _serviceService.GetServiceByIdAsync(id);
            return service != null ? Ok(service) : NotFound();
        }

        [HttpGet("servicio/{barberoId}")]
        public async Task<ActionResult<List<ServiceDTO>>> GetServicePorBarbero(int barberoId)
        {
            var services = await _serviceService.GetServicesPorBarberoAsync(barberoId);

            if (services == null || services.Count == 0)
                return NotFound($"No se encontraron barberos para la barbería con ID {barberoId}.");

            return Ok(services);
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
