using System.ComponentModel.DataAnnotations;

public class UsuarioCreateDTO
{
    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string Clave { get; set; }
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public int RoleId { get; set; }

}
