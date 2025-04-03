using CrudApi.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CrudApi.Interfaces
{
    public interface IBarberoService
    {
        Task<List<BarberoResponseDTO>> GetBarberoAsync(); // Usar ResponseDTO
        Task<BarberoResponseDTO?> GetBarberoByIdAsync(int id);
        Task<List<BarberoResponseDTO>> GetBarberosPorBarberiaAsync(int barberiaId);
        Task<BarberoResponseDTO> CreateBarberoAsync(BarberoCreateDTO barberoDto); // Cambiar a DTO
    }
}
