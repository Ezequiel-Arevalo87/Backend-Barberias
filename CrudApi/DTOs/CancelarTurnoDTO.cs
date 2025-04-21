public class CancelarTurnoDTO
{
    public int TurnoId { get; set; }
    public string Rol { get; set; } = string.Empty; // "Cliente" o "Barbero"
    public string Motivo { get; set; } = string.Empty;
    public bool Restaurar { get; set; } // Solo se aplica para Barbero
}
