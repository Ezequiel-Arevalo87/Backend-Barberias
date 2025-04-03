using CrudApi.DTOs;

public interface ITurnoService
{
    Task<List<TurnoDTO>> ObtenerTurnosAsync(int? barberoId);
    Task<TurnoDTO> CrearTurnoAsync(TurnoCreateDTO turnoCreateDTO);
    Task<TurnoDTO> NotificarTurnoAsync(TurnoDTO turnoDTO);
    Task EnviarNotificacionManualAsync(string token, TurnoDTO turnoDTO);
}
