using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

public class UsuarioCreateDTO
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Correo { get; set; } = string.Empty;

    [Required]
    [MaxLength(400)]
    public string Descripcion { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Clave { get; set; } = string.Empty;

    [Required]  // Asegura que el RoleId siempre sea proporcionado
    public int RoleId { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public string? Direccion { get; set; } = string.Empty;
    public string? Telefono { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? NombreBarberia { get; set; } = string.Empty;
}
