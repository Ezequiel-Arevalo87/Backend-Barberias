using System.ComponentModel.DataAnnotations;

public class BarberiaCreateDTO
{
    // Datos de Usuario
    public UsuarioCreateDTO Usuario { get; set; } = new UsuarioCreateDTO();

    // Datos propios de Barberia
    [Required]
    public string FotoBarberia { get; set; } = string.Empty;

    [Required]
    public int TipoDocumento { get; set; }

  

    [Required]
    [MaxLength(20)]
    public string NumeroDocumento { get; set; } = string.Empty;
}
