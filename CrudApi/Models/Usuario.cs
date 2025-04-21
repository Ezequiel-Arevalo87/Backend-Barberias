using CrudApi.Models;
using System.ComponentModel.DataAnnotations;

public class Usuario
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; }

    [Required]
    [EmailAddress]
    public string Correo { get; set; }

    [Required]
    [MinLength(6)]
    public string Clave { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public string? Direccion { get; set; }
    public string? Telefono { get; set; }

    // Relación con rol
    public int RoleId { get; set; }
    public Role Role { get; set; }



    public Cliente? Cliente { get; set; }
    public string? NotificationToken { get; set; }

    public Barbero? Barbero { get; set; }
    public Barberia? Barberia { get; set; }

}
