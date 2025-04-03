using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class SucursalBarberia
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; }

    [Required]
    [MaxLength(200)]
    public string Direccion { get; set; }

    [Required]
    [MaxLength(15)]
    public string Telefono { get; set; }

    [ForeignKey("Barberia")]
    public int BarberiaId { get; set; }
    public Barberia Barberia { get; set; }
}
