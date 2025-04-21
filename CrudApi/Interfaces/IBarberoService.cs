using CrudApi.DTOs;
using CrudApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudApi.Interfaces
{
    public interface IBarberoService
    {
        Task<List<BarberoResponseDTO>> GetBarberoAsync(); // Usar ResponseDTO
        Task<BarberoResponseDTO?> GetBarberoByIdAsync(int id);
        Task<List<BarberoResponseDTO>> GetBarberosPorBarberiaAsync(int barberiaId);
        Task<Barbero> UpdateBarberoAsync(int id, BarberoUpdateDTO barberoUpdateDTO);
        Task<int?> ObtenerBarberoIdDesdeUsuarioIdAsync(int usuarioId);
        Task<List<BarberoDTO>> ObtenerBarberosPorSucursalAsync(int sucursalId);
        Task<bool> RegistrarTokenFirebaseAsync(int barberoId, string nuevoToken);
        Task<BarberoResponseDTO> CreateBarberoAsync(BarberoCreateDTO barberoDto);
        Task<bool> RegistrarTokenFirebasePorUsuarioAsync(int usuarioId, string nuevoToken);
    }
}
