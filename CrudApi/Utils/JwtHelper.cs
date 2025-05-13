using CrudApi.Models;
using CrudApi.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;

namespace CrudApi.Utils
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public JwtHelper(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        public string GenerateToken(string userId, string email, string role, string? barberoId = null, string? barberiaId = null)
        {
            var secretKey = _config["JwtSettings:Key"];
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new Exception("⚠️ Error: La configuración de JWT (Key, Issuer o Audience) no está correctamente definida.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            if (!string.IsNullOrEmpty(barberoId))
                claims.Add(new Claim("barberoId", barberoId));

            if (!string.IsNullOrEmpty(barberiaId))
                claims.Add(new Claim("barberiaId", barberiaId));

            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string? GenerateToken(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));

            string roleName = usuario.Role?.Nombre ?? "Usuario";

            var barbero = _context.Barberos
                .Where(b => b.UsuarioId == usuario.Id)
                .Select(b => new { b.Id, b.BarberiaId })
                .FirstOrDefault();

            string? barberoId = barbero?.Id.ToString();
            string? barberiaId = barbero?.BarberiaId.ToString();

            return GenerateToken(usuario.Id.ToString(), usuario.Correo, roleName, barberoId, barberiaId);
        }
    }
}
