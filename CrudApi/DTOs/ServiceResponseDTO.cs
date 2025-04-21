public class ServiceResponseDTO
{
    public int Id { get; set; }
    public string Servicio { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Foto { get; set; }
    public decimal Precio { get; set; }
    public decimal? PrecioEspecial { get; set; }
    public int Tiempo { get; set; }
    public string? Observacion { get; set; }
    public int? Estado { get; set; }

    public int BarberoId { get; set; }
    public string BarberoNombre { get; set; } = string.Empty;
    public string? BarberoEmail { get; set; }

    public int? BarberiaId { get; set; }
    public string? BarberiaNombre { get; set; }

    // Opcional si usas sucursales
    public int? SucursalId { get; set; }
    public string? SucursalNombre { get; set; }
}
