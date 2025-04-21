namespace CrudApi.DTOs
{
    public class BarberoUpdateDTO
    {
     
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string FotoBarbero { get; set; }
        public int? Estado { get; set; }
        public int? SucursalId { get; set; }
        public string? NombreSucursal { get; set; } // opcional para mostrar
    }

}
