namespace CrudApi.DTOs
{
    public class BarberiaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;

        public string FotoBarberia { get; set; } = string.Empty;

        public int TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;  
        
        public string Email { get; set; } = string.Empty;

    }
}
