using System.ComponentModel.DataAnnotations;

public class BarberoCreateDTO
{
    [Required]
    public string Nombre { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    [Required]
    public int RoleId { get; set; } // Ej: 2 para Barbero

    [Required]
    public int TipoDocumento { get; set; }

    [Required]
    public string NumeroDocumento { get; set; }

    public int Estado { get; set; }

    public string? FotoBarbero { get; set; }

    public int? SucursalId { get; set; }
    public string? NombreSucursal { get; set; } // opcional para mostrar

    [Required]
    public int BarberiaId { get; set; }
}
