public interface IClienteService
{
    Task<ClienteDTO> RegistrarCliente(ClienteRegistroDTO clienteDto);
    Task<ClienteDTO> ObtenerClientePorEmail(string email);
}
