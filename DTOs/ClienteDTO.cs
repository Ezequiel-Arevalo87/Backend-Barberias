public class ClienteRegistroDTO
{
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime FechaNacimiento { get; set; }
}

public class ClienteDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public int CantidadReservas { get; set; }
}
