using CrudApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Barberia
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; }

    [Required]
    public int TipoDocumentoId { get; set; }

    [ForeignKey("TipoDocumentoId")]
    public TipoDocumento TipoDocumento { get; set; }

    [Required]
    [MaxLength(20)]
    public string NumeroDocumento { get; set; }

    [Required]
    [MaxLength(200)]
    public string Direccion { get; set; }

    [Required]
    public string? FotoBarberia { get; set; } 

    [Required]
    [MaxLength(15)]
    public string Telefono { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    public Role? Role { get; set; }

    public ICollection<Barbero> Barberos { get; set; } = new List<Barbero>();
    public ICollection<SucursalBarberia> Sucursales { get; set; } = new List<SucursalBarberia>();
    public ICollection<HorarioBarberia> Horarios { get; set; } = new List<HorarioBarberia>();
}
