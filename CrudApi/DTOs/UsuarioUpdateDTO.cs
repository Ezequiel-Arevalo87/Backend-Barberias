namespace CrudApi.DTOs
{
    public class UsuarioUpdateDTO
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string? Clave { get; set; } // Opcional, solo se actualiza si el usuario la envía
    }
}
