using CrudApi.DTOs;
using Microsoft.AspNetCore.Mvc;
using CrudApi.Services;
using TuProyectoNamespace.Services;
using Microsoft.EntityFrameworkCore;
using CrudApi.Data;

[Route("api/[controller]")]
[ApiController]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;
    private readonly EmailService _emailService;
    private readonly ApplicationDbContext _context;

    public ClientesController(IClienteService clienteService, EmailService emailService, ApplicationDbContext context)
    {
        _clienteService = clienteService;
        _emailService = emailService;
        _context = context;
    }

    [HttpPost("registro")]
    public async Task<IActionResult> RegistrarCliente([FromBody] ClienteRegistroDTO clienteDto)
    {
        try
        {
            var cliente = await _clienteService.RegistrarCliente(clienteDto);

            // 🔥 Construir URL para confirmar correo
            var urlConfirmacion = $"https://backend-barberias-1.onrender.com/api/Clientes/confirmar-correo?email={clienteDto.Email}";

            var asunto = "Bienvenido a BarberShop ✂️";
            var cuerpo = $@"
    <div style='text-align: center;'>
        <img src='https://i.ibb.co/Q3zFcwff/logo-2.png' alt='Logo BarberShop' style='width: 150px; margin-bottom: 20px;' />
        <h1>¡Hola {clienteDto.Nombre}!</h1>
        <p>Gracias por registrarte en <b>BarberShop</b>.</p>
        <p>Tu cuenta está casi lista. Solo falta confirmar tu correo electrónico.</p>
        <br />
        <a href='https://confirmacion-barberiashop.vercel.app/?email={clienteDto.Email}' style='background-color: #f3973c; padding: 10px 20px; color: white; text-decoration: none; border-radius: 8px; font-size: 16px;'>
            Confirmar mi correo
        </a>
        <p style='margin-top: 20px;'>Si no solicitaste esta cuenta, puedes ignorar este mensaje.</p>
        <p>El equipo de BarberShop ✂️</p>
    </div>";


            await _emailService.SendEmailAsync(clienteDto.Email, asunto, cuerpo);

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

    [HttpGet("confirmar-correo")]
    public async Task<IActionResult> ConfirmarCorreo(string email)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Usuario)
            .FirstOrDefaultAsync(c => c.Usuario.Correo == email);

        if (cliente == null)
            return NotFound("Cliente no encontrado.");

        cliente.Verificado = true;
        await _context.SaveChangesAsync();

        // ✅ Mostrar una respuesta más amigable
        var html = $@"
            <html>
                <head>
                    <title>Correo confirmado</title>
                </head>
                <body style='font-family:sans-serif;text-align:center;padding-top:50px;'>
                    <h1 style='color:#13487a;'>¡Tu correo ha sido confirmado exitosamente! 🎉</h1>
                    <p>Ya puedes disfrutar de todos los servicios de <b>BarberShop</b>.</p>
                </body>
            </html>
        ";

        return Content(html, "text/html");
    }
}
