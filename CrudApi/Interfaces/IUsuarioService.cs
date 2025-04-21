using CrudApi.DTOs;
using CrudApi.Models;

namespace CrudApi.Interfaces
{
    public interface IUsuarioService
    {
        Task<List<UsuarioDTO>> GetUsuariosAsync();
        Task<UsuarioDTO?> GetUsuarioByIdAsync(int id);
        Task<Usuario> CreateUsuarioAsync(UsuarioCreateDTO usuarioDto); // Cambio aquí
        Task<Usuario> UpdateUsuarioAsync(int id, UsuarioUpdateDTO  usuarioUpdateDTO ); // Cambio aquí
        Task<bool> DeleteUsuarioAsync(int id);
        Task SaveChangesAsync();
    }
}
