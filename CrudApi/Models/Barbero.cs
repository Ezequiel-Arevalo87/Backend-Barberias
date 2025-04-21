using CrudApi.Models;

public class Barbero
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }

    public int BarberiaId { get; set; }
    public Barberia Barberia { get; set; }

    public int TipoDocumentoId { get; set; }
    public string NumeroDocumento { get; set; }

    public int? Estado { get; set; } 

    public string? FotoBarbero { get; set; }
    public string? NotificationToken { get; set; }

    public int? SucursalId { get; set; }
    public SucursalBarberia? Sucursal { get; set; }

    public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
}
