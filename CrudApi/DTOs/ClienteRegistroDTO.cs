public class ClienteRegistroDTO
{
    public string Nombre { get; set; }

    public string Apellido { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public string? Direccion { get; set; }

    public string? Telefono { get; set; }

    public int RoleId { get; set; } = 3; // Cliente
}
