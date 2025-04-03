public interface IHorarioService
{
    Task<IEnumerable<HorarioBarberiaDTO>> GetHorariosAsync(int barberiaId);
    Task<HorarioBarberiaDTO?> GetHorarioByIdAsync(int id);
    Task<HorarioBarberiaDTO> CrearHorarioAsync(HorarioBarberiaDTO horarioBarberiaDTO);
    Task<bool> ActualizarHorarioAsync(HorarioBarberiaDTO horarioBarberiaDTO);
    Task<bool> EliminarHorarioAsync(int id);
}
