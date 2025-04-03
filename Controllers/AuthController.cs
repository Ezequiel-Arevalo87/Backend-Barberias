using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Utils;
using System.Threading.Tasks;
using CrudApi.Models;

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
                //return Unauthorized(new { mensaje = "Correo o contraseña incorrectos" });
                return Ok(new { mensaje = "Datos no encontrados" });

            }

            // Obtener el nombre del rol (por defecto "Usuario" si es null)
            string roleName = usuario.Role?.Nombre ?? "Usuario";

            var token = _jwtHelper.GenerateToken(usuario.Id.ToString(), usuario.Correo, roleName);

            return Ok(new { token });
        }

        [HttpPost("loginBarberia")]
        public async Task<IActionResult> loginBarberia([FromBody] LoginDTO loginDTO)
        {
            var barberia = await _context.Barberias
                .Include(br => br.Role) // Asegurar que se cargue el rol
                .FirstOrDefaultAsync(br => br.Email == loginDTO.Correo);

            if (barberia == null || !PasswordHasher.VerifyPassword(loginDTO.Clave, barberia.Password))
            {
                return Ok(new { mensaje = "Datos no encontrados" });
            }

            // Obtener el nombre del rol (por defecto "Usuario" si es null)
            string roleName = barberia.Role?.Nombre ?? "barberia";

            var token = _jwtHelper.GenerateToken(barberia.Id.ToString(), barberia.Email, roleName);

            return Ok(new { token });
        }

        [HttpPost("loginBarbero")]
        public async Task<IActionResult> loginBarbero([FromBody] LoginDTO loginDTO)
        {
            var barbero = await _context.Barberos
                .Include(b => b.Role) // Asegurar que se cargue el rol
                .FirstOrDefaultAsync(b => b.Email == loginDTO.Correo);

            if (barbero == null || !PasswordHasher.VerifyPassword(loginDTO.Clave, barbero.Password))
            {
                return Ok(new { mensaje = "Datos no encontrados" });
            }

            // Obtener el nombre del rol (por defecto "Usuario" si es null)
            string roleName = barbero.Role?.Nombre ?? "barbero";

            var token = _jwtHelper.GenerateToken(barbero.Id.ToString(), barbero.Email, roleName);

            return Ok(new { token });
        }
    }
}
