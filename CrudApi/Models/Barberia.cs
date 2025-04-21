public class Barberia
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }
    public Usuario Usuario { get; set; }

    public int TipoDocumento { get; set; }
    public string NumeroDocumento { get; set; }

    public string? FotoBarberia { get; set; }

    public int? Estado { get; set; } 

    public ICollection<SucursalBarberia> Sucursales { get; set; } = new List<SucursalBarberia>();
    public ICollection<Barbero> Barberos { get; set; } = new List<Barbero>();
    public ICollection<HorarioBarberia> Horarios { get; set; } = new List<HorarioBarberia>();
}
