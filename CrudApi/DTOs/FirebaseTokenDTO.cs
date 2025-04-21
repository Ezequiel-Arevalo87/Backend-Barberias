namespace CrudApi.DTOs
{
    public class FirebaseTokenDTO
    {
        public int ClienteId { get; set; }  // ⬅️ CAMBIADO de UsuarioId a ClienteId
        public string Token { get; set; } = string.Empty;
    }
}


