using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudApi.Models
{
    public class Turno
    {
        [Key]
        public int TurnoId { get; set; } // ✅ Nombre correcto para la clave primaria

        [Required]
        public DateTime FechaHora { get; set; }

        public int BarberoId { get; set; }
        [ForeignKey("BarberoId")]
        public Barbero? Barbero { get; set; }

        public int ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        public int ServicioId { get; set; }
        [ForeignKey("ServicioId")]
        public Service? Servicio { get; set; }

        public TimeSpan HoraInicio { get; set; }
        public DateTime FechaHoraInicio { get; set; } // Unificado

        public TimeSpan Duracion { get; set; }

        public string Estado { get; set; } = "Pendiente";
    }
}
