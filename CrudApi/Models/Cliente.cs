using CrudApi.Models;

public class Cliente
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string Password { get; set; } // Hash de la contraseña
    public DateTime FechaNacimiento { get; set; }
    public int RoleId { get; set; } = 3; // Cliente

    // Relación con Reservas

    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
