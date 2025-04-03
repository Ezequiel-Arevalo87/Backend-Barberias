using CrudApi.DTOs;
using CrudApi.Models;

namespace CrudApi.Interfaces
{
    public interface IBarberiaService
    {
        Task<List<BarberiaDTO>> GetBarberiaAsync();
        Task<BarberiaDTO?> GetBarberiaByIdAsync(int id);
        public  Task<Barberia> CreateBarberiaAsync(BarberiaCreateDTO barberiaDto);

        Task<Barberia> UpdateBarberiaAsync(int id, BarberiaUpdateDTO barberiaUpdateDTO); // Cambio aquí
        Task<bool> DeleteBarberiaAsync(int id);
        Task SaveChangesAsync();
    }
}
