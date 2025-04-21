public class BarberiaDTO
{
    public int Id { get; set; }
    // Se obtiene del Usuario
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string? Direccion { get; set; }
    public string? Telefono { get; set; }

    // Datos propios de Barberia
    public int TipoDocumento { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string FotoBarberia { get; set; } = string.Empty;
    public int? Estado { get; set; }
    public string? NombreBarberia { get; set; } 

    public List<SucursalBarberiaDTO>? Sucursales { get; set; }
}
