public class CrearHorarioBloqueadoDTO
{
    public int BarberoId { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string? Motivo { get; set; }
}
