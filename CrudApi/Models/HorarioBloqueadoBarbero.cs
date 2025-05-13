using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class HorarioBloqueadoBarbero
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BarberoId { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    [Required]
    public TimeSpan HoraInicio { get; set; }

    [Required]
    public TimeSpan HoraFin { get; set; }

    [MaxLength(200)]
    public string? Motivo { get; set; }

    [ForeignKey("BarberoId")]
    public Barbero Barbero { get; set; } = null!;
}
