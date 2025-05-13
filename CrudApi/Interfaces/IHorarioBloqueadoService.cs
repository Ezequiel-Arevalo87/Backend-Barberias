using System.Threading.Tasks;

public interface IHorarioBloqueadoService
{
    Task<bool> CrearBloqueoAsync(CrearHorarioBloqueadoDTO dto);
}
