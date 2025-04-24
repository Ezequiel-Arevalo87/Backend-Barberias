using CrudApi.Models;

public class TurnoDTO
{
    public int Id { get; set; }
    public int BarberoId { get; set; }
    public int ServicioId { get; set; }
    public int ClienteId { get; set; }
    public DateTime Fecha { get; set; } // Asegúrate de que esté presente
    public DateTime FechaHoraInicio { get; set; } // Unificado
    public DateTime HoraFin { get; set; }
    public TimeSpan Duracion { get; set; }
    public EstadoTurno Estado { get; set; }
    public string ClienteNombre { get; set; }
    public string ClienteApellido { get; set; }
    public string ClienteEmail { get; set; }
    public DateTime ClienteFechaNacimiento { get; set; }
    public string ServicioNombre { get; set; }
    public string ServicioDescripcion { get; set; }
    public decimal ServicioPrecio { get; set; }
    public decimal? ServicioPrecioEspecial { get; set; }
    public string BarberoNombre { get; set; }
    public string MotivoCancelacion { get; set; }
    public string BarberiaNombre { get; set; } // ✅ Nuevo campo


}
