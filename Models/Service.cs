using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;  // Necesario para [Index]

namespace CrudApi.Models
{
   

    [Index(nameof(BarberoId))]  // Índice para mejorar rendimiento en consultas
    public class Service  // Si quieres más consistencia con el idioma, cámbialo a "Servicio"
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]  // Limitar el tamaño del nombre del servicio
        public string Servicio { get; set; } = string.Empty;

        [Required]
        public int Estado { get; set; }

        public string? Descripcion { get; set; }

        public string? Foto { get; set; }  // URL o base64

        public decimal? PrecioEspecial { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]  // Definir precisión en la BD
        public decimal Precio { get; set; }

        [Required]
        public int Tiempo { get; set; } // Minutos estimados

        public string? Observacion { get; set; }

        // Relación con Barbero (uno a muchos)
        [ForeignKey("Barbero")]
        public int BarberoId { get; set; }
        public Barbero? Barbero { get; set; }

 
        public ICollection<Turno> Turnos { get; set; } = new List<Turno>();
    }
}
