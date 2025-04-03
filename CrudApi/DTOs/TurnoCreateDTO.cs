namespace CrudApi.DTOs
{
    public class TurnoCreateDTO
    {
        public int TurnoId { get; set; } // ✅ Nombre correcto para la clave primaria
        public int BarberoId { get; set; }
        public int ClienteId { get; set; }
        public int ServicioId { get; set; }
        public DateTime FechaHora { get; set; }
        
        public DateTime FechaHoraInicio { get; set; }
        public int Duracion { get; set; } // Ahora es un entero (minutos)
        public string Estado { get; set; }

    }
}
