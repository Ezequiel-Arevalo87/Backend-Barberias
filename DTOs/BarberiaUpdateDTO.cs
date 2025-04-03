using System.ComponentModel.DataAnnotations;

public class BarberiaUpdateDTO
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(400)]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    public string? FotoBarberia { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public string? Direccion { get; set; }
    public string? Telefono { get; set; }

    [MaxLength(100)]
    public string? NombreBarberia { get; set; }
}
