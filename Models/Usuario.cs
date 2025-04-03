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

        // 🔹 Campos adicionales que serán obligatorios solo si RoleId == 1
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }

        public string? Descripcion { get; set; }


        [MaxLength(100)]
        public string? NombreBarberia { get; set; }


        // 🔥 Implementamos validación condicional
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RoleId == 1) // Si el rol es 1, estos campos deben ser obligatorios
            {
                if (string.IsNullOrWhiteSpace(Direccion))
                    yield return new ValidationResult("La dirección es obligatoria para RoleId 1", new[] { nameof(Direccion) });

                if (string.IsNullOrWhiteSpace(Telefono))
                    yield return new ValidationResult("El teléfono es obligatorio para RoleId 1", new[] { nameof(Telefono) });

                if (string.IsNullOrWhiteSpace(Descripcion))
                    yield return new ValidationResult("La descripción es obligatorio para RoleId 1", new[] { nameof(Descripcion) });
                if (string.IsNullOrWhiteSpace(NombreBarberia))
                    yield return new ValidationResult("El Nombre de la barberia es obligatorio para RoleId 1", new[] { nameof(NombreBarberia) });
            }
        }
    }
}
