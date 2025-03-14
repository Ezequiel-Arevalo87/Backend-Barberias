using CrudApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CrudApi.Utils
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public string GenerateToken(string userId, string email, string role)
        {
            var secretKey = _config["JwtSettings:Key"];
            var issuer = _config["JwtSettings:Issuer"];
            var audience = _config["JwtSettings:Audience"];

            if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
            {
                throw new Exception("⚠️ Error: La configuración de JWT (Key, Issuer o Audience) no está correctamente definida.");
            }

            var key = Encoding.UTF8.GetBytes(secretKey);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Role, role) // Agregado el rol del usuario
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                Issuer = issuer,  // ✅ Agregado Issuer
                Audience = audience,  // ✅ Agregado Audience
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        internal string? GenerateToken(Usuario usuario)
        {
            if (usuario == null) throw new ArgumentNullException(nameof(usuario));

            string roleName = usuario.Role?.Nombre ?? "Usuario"; // Usa "Usuario" si no tiene rol asignado

            return GenerateToken(usuario.Id.ToString(), usuario.Correo, roleName);
        }
    }
}
