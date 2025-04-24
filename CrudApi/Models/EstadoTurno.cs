namespace CrudApi.Models
{
    public enum EstadoTurno
    {
        Pendiente,    // Cuando el cliente agenda el turno
        EnProceso,    // Cuando llega la hora del turno
        Cerrado,      // Cuando termina automáticamente según duración
        Cancelado,    // Si lo cancela cliente o barbero
        Disponible,   // Si se libera y vuelve a estar disponible para otros
    }
}
