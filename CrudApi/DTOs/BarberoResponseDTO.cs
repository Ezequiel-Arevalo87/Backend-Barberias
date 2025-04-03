public class BarberoResponseDTO
{
    public int Id { get; set; }
    public int BarberiaId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int TipoDocumentoId { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? NombreBarberia { get; set; }
}
