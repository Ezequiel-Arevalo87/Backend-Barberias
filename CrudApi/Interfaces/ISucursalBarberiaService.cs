public interface ISucursalBarberiaService
{
    Task<List<SucursalBarberiaDTO>> GetAllAsync();
    Task<SucursalBarberiaDTO?> GetByIdAsync(int id);
    Task<SucursalBarberiaDTO> CreateAsync(SucursalBarberiaDTO dto);
    Task<SucursalBarberiaDTO?> UpdateAsync(int id, SucursalBarberiaDTO dto);
    Task<bool> DeleteAsync(int id);
}
