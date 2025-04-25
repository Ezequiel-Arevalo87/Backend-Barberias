using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Utils;
using CrudApi.Services; // ✅ Agregamos el namespace donde estará EmailService
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TuProyectoNamespace.Services;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtHelper _jwtHelper;
    private readonly EmailService _emailService; // ✅ Inyectamos el nuevo servicio de email

    public AuthController(ApplicationDbContext context, JwtHelper jwtHelper, EmailService emailService)
    {
        _context = context;
        _jwtHelper = jwtHelper;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO loginDto)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.Cliente)
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Correo == loginDto.Correo);

        if (usuario == null || !PasswordHasher.VerifyPassword(loginDto.Clave, usuario.Clave))
        {
            return Unauthorized(new { message = "Credenciales inválidas" });
        }

        if (usuario.Role == null)
        {
            return BadRequest(new { message = "Este usuario no tiene un rol asignado." });
        }

        // 🔍 Buscamos si este usuario está asociado como barbero
        var barbero = await _context.Barberos.FirstOrDefaultAsync(b => b.UsuarioId == usuario.Id);

        var token = _jwtHelper.GenerateToken(usuario);

        return Ok(new
        {
            token,
            usuarioId = usuario.Id,
            role = usuario.Role.Nombre,
            clienteId = usuario.Cliente?.Id,
            barberoId = barbero?.Id
        });
    }

    // 🔥 Nuevo endpoint para probar envío de correos
    [HttpGet("probar-correo")]
    public async Task<IActionResult> ProbarCorreo()
    {
        var toEmail = "correo@destino.com"; // 📬 Pon aquí tu correo de prueba
        var subject = "Prueba de Correo desde Barbería";
        var body = "<h1>¡Bienvenido!</h1><p>Este es un correo de prueba de tu backend funcionando 🎉</p>";

        var enviado = await _emailService.SendEmailAsync(toEmail, subject, body);

        if (enviado)
            return Ok("✅ Correo enviado correctamente.");
        else
            return StatusCode(500, "❌ Error enviando correo.");
    }
}
