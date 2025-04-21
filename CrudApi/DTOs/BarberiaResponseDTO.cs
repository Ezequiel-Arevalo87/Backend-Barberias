public class BarberiaResponseDTO
{
    public int Id { get; set; }


    public string Nombre { get; set; }
    public string Correo { get; set; }
    public string FotoBarberia { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public string NombreUsuario { get; set; }

    public int Estado { get; set; }
    public int UsuarioId { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public int TipoDocumento { get; set; }

    public List<SucursalBarberiaDTO> Sucursales { get; set; } = new();
}
