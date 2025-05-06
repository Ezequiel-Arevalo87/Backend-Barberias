using CrudApi.DTOs;

public interface ITurnoService
{
    Task<List<TurnoDTO>> ObtenerTurnosAsync(int? barberoId);
    Task<TurnoDTO> CrearTurnoAsync(TurnoCreateDTO turnoCreateDTO);
    Task<TurnoDTO> NotificarTurnoAsync(TurnoDTO turnoDTO);
    Task EnviarNotificacionManualAsync(string token, TurnoDTO turnoDTO, bool paraCliente);
    Task<bool> CancelarTurnoAsync(CancelarTurnoDTO dto); // ✅ nuevo método
    Task<List<TurnoDTO>> ObtenerTurnosDelBarberoAsync(int barberoId, FiltroReporteTurnoDTO filtro);
    Task<List<HorarioTurnoDTO>> ObtenerHorariosReservadosPorBarberoYFechaAsync(int barberoId, DateTime fecha);
    Task<List<TurnoDTO>> ObtenerHistorialPorClienteAsync(int clienteId);


}
