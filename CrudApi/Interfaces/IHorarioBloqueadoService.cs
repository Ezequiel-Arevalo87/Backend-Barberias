using CrudApi.DTOs;
using System.Threading.Tasks;

public interface IHorarioBloqueadoService
{
    Task<bool> CrearBloqueoAsync(CrearHorarioBloqueadoDTO dto);
    Task<List<HorarioTurnoDTO>> ObtenerBloqueosAsync(int barberoId, DateTime fecha);

}
