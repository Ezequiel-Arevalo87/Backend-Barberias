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
    [MinLength(6)]
    public string Clave { get; set; } = string.Empty;

    [Required]  // Asegura que el RoleId siempre sea proporcionado
    public int RoleId { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
