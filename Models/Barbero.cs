using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudApi.Models
{
    public class Barbero
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        public int TipoDocumentoId { get; set; }

        [ForeignKey("TipoDocumentoId")]
        public TipoDocumento? TipoDocumento { get; set; }  // ⚠️ Hacerlo nullable si no siempre se carga

        [Required]
        [MaxLength(20)]
        public string NumeroDocumento { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Direccion { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public int RoleId { get; set; }

        [ForeignKey("RoleId")]
        public Role? Role { get; set; }

        [Required]
        public int BarberiaId { get; set; }

        [ForeignKey("BarberiaId")]
        public Barberia? Barberia { get; set; }  // ⚠️ Hacerlo nullable por precaución

        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
