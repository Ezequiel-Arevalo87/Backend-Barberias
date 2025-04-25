using CrudApi.Models;

public class Cliente
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }  // 👈 Relación obligatoria

    public string Apellido { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public int? Estado { get; set; } = 1;

    public string? NotificationToken { get; set; }



    public bool Verificado { get; set; } = false; // 👈 Agregas este campo

    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();

 

}
