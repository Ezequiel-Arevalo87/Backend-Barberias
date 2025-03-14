using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudApi.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string Clave { get; set; } = string.Empty;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Agregamos el campo de RoleId como clave foránea
        [Required]
        public int RoleId { get; set; }

        // Propiedad de navegación para la relación con Role
        [ForeignKey("RoleId")]
        public Role? Role { get; set; }
    }
}
