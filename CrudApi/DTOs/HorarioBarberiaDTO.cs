public class HorarioBarberiaDTO
{
    public int Id { get; set; }
    public int BarberiaId { get; set; }
    public string DiaSemana { get; set; }
    public bool EsFestivo { get; set; }
    public bool Abierto { get; set; }
    public string? HoraInicio { get; set; } = string.Empty; // Se recibe como string
    public string? HoraFin { get; set; } = string.Empty; // Se recibe como string

  
}