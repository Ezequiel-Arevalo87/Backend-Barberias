using System;

public class UsuarioDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    
    // Excluir la clave por seguridad en la transferencia de datos
    public string? Rol { get; set; } 

    private DateTime _fechaRegistro;
    public DateTime FechaRegistro
    {
        get => TimeZoneInfo.ConvertTimeFromUtc(_fechaRegistro, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time"));
        set => _fechaRegistro = value;
    }
}
