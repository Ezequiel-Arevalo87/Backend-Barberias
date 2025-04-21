public class SucursalBarberiaDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string? FotoSucursal { get; set; }
    public int BarberiaId { get; set; }
    public int? TipoDocumentoId { get; set; }
    public string? NumeroDocumento { get; set; }
    public int? Estado { get; set; }

    //public int SucursalId { get; set; }
   
}
