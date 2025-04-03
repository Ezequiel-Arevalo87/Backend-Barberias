using System.ComponentModel.DataAnnotations;

namespace CrudApi.DTOs
{
    public class BarberoCreateDTO
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public int TipoDocumento { get; set; }

        [Required]
        public string NumeroDocumento { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        [Required]
        public int BarberiaId { get; set; }
    }
}
