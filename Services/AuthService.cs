using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CrudApi.Data;
using CrudApi.Interfaces;
using CrudApi.Models;
using CrudApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CrudApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, JwtHelper jwtHelper, IConfiguration configuration)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _configuration = configuration;
        }

        /// <summary>
        /// Registra un nuevo usuario en la base de datos.
        /// </summary>
        public async Task<Usuario?> Register(string correo, string password)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Correo == correo))
                throw new Exception("El correo ya está registrado.");

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var usuario = new Usuario
            {
                Correo = correo,
                Clave = hashedPassword
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        /// <summary>
        /// Valida las credenciales del usuario y genera un token JWT si son correctas.
        /// </summary>
        public async Task<string?> Login(string email, string password)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.Clave))
                throw new Exception("Credenciales inválidas.");

            return _jwtHelper.GenerateToken(usuario);
        }

        /// <summary>
        /// Valida un token JWT.
        /// </summary>
        public ClaimsPrincipal? ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true
                }, out _);

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
