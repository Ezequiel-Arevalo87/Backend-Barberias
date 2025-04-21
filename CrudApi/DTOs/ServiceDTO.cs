using CrudApi.Models;

public class ServiceDTO
{
    public int Id { get; set; }
    public string Servicio { get; set; }
    public int? Estado { get; set; }
    public string Descripcion { get; set; }
    public string Foto { get; set; }
    public decimal? PrecioEspecial { get; set; }  // ✅ Ahora es nullable
    public decimal Precio { get; set; }
    public int Tiempo { get; set; }
    public string Observacion { get; set; }
    public int BarberoId { get; set; }
}
