using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

public class BarberiaCreateDTO
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string FotoBarberia { get; set; }

    [Required]
    public int TipoDocumento { get; set; }

    [Required(ErrorMessage = "El número de documento es obligatorio")]
    [MaxLength(20)]
    public string NumeroDocumento { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    public string? Direccion { get; set; }
    public string? Telefono { get; set; }

    [Required]
    public int RoleId { get; set; }

    // No es necesario en un DTO de creación incluir la navegación
    // El Role se maneja internamente desde el backend
}
