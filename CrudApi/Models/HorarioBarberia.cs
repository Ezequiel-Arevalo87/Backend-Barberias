using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class HorarioBarberia
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BarberiaId { get; set; }

    [Required]
    [MaxLength(20)]
    public string DiaSemana { get; set; } = string.Empty;

    [Required]
    public bool? EsFestivo { get; set; } = false;

    [Required]
    public bool? Abierto { get; set; } = true;

    public TimeSpan HoraInicio { get; set; } // TimeSpan para manejar horas
    public TimeSpan HoraFin { get; set; } // TimeSpan para manejar horas

    [ForeignKey("BarberiaId")]
    public Barberia Barberia { get; set; } = null!;
}