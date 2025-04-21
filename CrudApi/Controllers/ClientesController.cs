using CrudApi.DTOs;
using Microsoft.AspNetCore.Mvc;


[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> RegistrarCliente([FromBody] ClienteRegistroDTO clienteDto)
    {
        try
        {
            var cliente = await _clienteService.RegistrarCliente(clienteDto);
            return CreatedAtAction(nameof(RegistrarCliente), new { id = cliente.Id }, cliente);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("email/{email}")]
    public async Task<IActionResult> ObtenerClientePorEmail(string email)
    {
        var cliente = await _clienteService.ObtenerClientePorEmail(email);
        if (cliente == null) return NotFound();
        return Ok(cliente);
    }

    [HttpPost("registrar-token")]
    public async Task<IActionResult> RegistrarToken([FromBody] FirebaseTokenDTO dto)
    {
        var result = await _clienteService.RegistrarTokenFirebaseAsync(dto.ClienteId, dto.Token);
        if (!result) return NotFound("Cliente no encontrado");

        return Ok("Token registrado correctamente para cliente");
    }



}
