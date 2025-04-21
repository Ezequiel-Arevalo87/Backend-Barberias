using System.ComponentModel.DataAnnotations;

public class BarberiaUpdateDTO
{
    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    // Si no queremos actualizar Email, Password y Role, no se incluyen aquí.
    // Se actualizan solo los datos de Usuario: Nombre, Dirección y Teléfono.
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }


    // Datos propios de Barberia
    [Required]
    public string FotoBarberia { get; set; } = string.Empty;
}
