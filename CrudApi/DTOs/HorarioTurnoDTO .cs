namespace CrudApi.DTOs
{
    public class HorarioTurnoDTO
    {
        public string Inicio { get; set; } = string.Empty; // Ej: "17:00"
        public int Duracion { get; set; } // Ej: 30 minutos
    }
}
