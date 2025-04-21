using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly JwtHelper _jwtHelper;

    public AuthController(ApplicationDbContext context, JwtHelper jwtHelper)
    {
        _context = context;
        _jwtHelper = jwtHelper;
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
            barberoId = barbero?.Id // ✅ Esto es lo nuevo que necesitas
        });
    }
}
