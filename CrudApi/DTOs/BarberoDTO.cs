using System.ComponentModel.DataAnnotations;

namespace CrudApi.DTOs
{
    public class BarberoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int TipoDocumento { get; set; }
        public string NumeroDocumento { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string FotoBarbero { get; set; }
        public int? Estado { get; set; }

        public int UsuarioId { get; set; }
        public int? SucursalId { get; set; }
        public string Telefono { get; set; } = string.Empty;
        [Required]  // Relación con la Barbería
        public int BarberiaId { get; set; }
       public string  NombreBarberia { get; set; }
    }
}
