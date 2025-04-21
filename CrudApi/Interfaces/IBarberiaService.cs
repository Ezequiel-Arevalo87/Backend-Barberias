using CrudApi.DTOs;
using CrudApi.Models;

public interface IBarberiaService
{

    Task<List<BarberiaResponseDTO>> GetBarberiasAsync(int usuarioId, int roleId);

    Task<BarberiaResponseDTO?> GetBarberiaByIdAsync(int id);




    Task<BarberiaResponseDTO> CreateBarberiaAsync(BarberiaCreateDTO barberiaDto);
    Task<Barberia?> UpdateBarberiaAsync(int id, BarberiaUpdateDTO barberiaDto);
    Task<bool> DeleteBarberiaAsync(int id);
    Task<List<BarberiaResponseDTO>> ObtenerTodasAsync(); // público (cliente)
    Task<List<BarberiaResponseDTO>> ObtenerPorUsuarioId(int usuarioId); // admin
    Task SaveChangesAsync();
}
