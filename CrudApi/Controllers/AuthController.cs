using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Utils;
using System.Threading.Tasks;

namespace CrudApi.Controllers
{
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
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Role) // Asegurar que se cargue el rol
                .FirstOrDefaultAsync(u => u.Correo == loginDTO.Correo);

            if (usuario == null || !PasswordHasher.VerifyPassword(loginDTO.Clave, usuario.Clave))
            {
                return Unauthorized(new { mensaje = "Correo o contraseña incorrectos" });
            }

            // Obtener el nombre del rol (por defecto "Usuario" si es null)
            string roleName = usuario.Role?.Nombre ?? "Usuario";

            var token = _jwtHelper.GenerateToken(usuario.Id.ToString(), usuario.Correo, roleName);

            return Ok(new { token });
        }
    }
}
