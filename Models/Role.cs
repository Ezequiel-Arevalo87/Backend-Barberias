using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudApi.Models
{
    public class Role
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        // Relación con Usuarios
        public ICollection<Usuario>? Usuarios { get; set; }

        public ICollection<Barbero>? Barberos { get; set; }

        public ICollection<Barberia>? Barberias { get; set; }
    }
}
