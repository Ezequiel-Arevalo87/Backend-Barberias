using System.Security.Claims;
using CrudApi.Models;

namespace CrudApi.Interfaces
{
    public interface IAuthService
    {
        Task<Usuario?> Register(string correo, string password);
        Task<string?> Login(string correo, string password);
        ClaimsPrincipal? ValidateJwtToken(string token);
    }
}
