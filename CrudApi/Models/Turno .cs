using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CrudApi.Models
{
    public class Turno
    {
        public int Id { get; set; }
        public int BarberoId { get; set; }
        public int ClienteId { get; set; }
        public int ServicioId { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime FechaHoraInicio { get; set; }
        public DateTime HoraFin { get; set; }
        public TimeSpan Duracion { get; set; }
        public EstadoTurno Estado { get; set; }


        // 🆕 Nueva propiedad para guardar motivo de cancelación
        public string? MotivoCancelacion { get; set; }

        public Cliente Cliente { get; set; }
        public Service Servicio { get; set; }
        public Barbero Barbero { get; set; }
        public bool Notificado { get; set; } = false;

    }
}
