using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CrudApi.Models
{
    [Index(nameof(BarberoId))] // Mejora de rendimiento en consultas por barbero
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty; // ✅ Renombrado a "Nombre" en lugar de "Servicio" para claridad

        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PrecioEspecial { get; set; }

        public int Tiempo { get; set; } // Tiempo en minutos

        public string? Foto { get; set; }

        public string? Observacion { get; set; }

        [Required]
        public int? Estado { get; set; } = 1; // 1: Activo, 0: Inactivo

        // 🔗 Relación con Barbero
        [ForeignKey("Barbero")]
        public int BarberoId { get; set; }
        public Barbero? Barbero { get; set; }

        // 🔗 Relación con Turnos
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
