using CrudApi.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;


public class ClienteService : IClienteService
{
    private readonly ApplicationDbContext _context;

    public ClienteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClienteDTO> RegistrarCliente(ClienteRegistroDTO clienteDto)
    {
        if (await _context.Clientes.AnyAsync(c => c.Email == clienteDto.Email))
            throw new Exception("El email ya está registrado");

        var cliente = new Cliente
        {
            Nombre = clienteDto.Nombre,
            Apellido = clienteDto.Apellido,
            Email = clienteDto.Email,
            Password = clienteDto.Password,
            FechaNacimiento = clienteDto.FechaNacimiento
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return new ClienteDTO
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            Email = cliente.Email,
            FechaNacimiento = cliente.FechaNacimiento,
            CantidadReservas = 0
        };
    }

    public async Task<ClienteDTO> ObtenerClientePorEmail(string email)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Turnos)
            .FirstOrDefaultAsync(c => c.Email == email);

        if (cliente == null)
            return null;

        return new ClienteDTO
        {
            Id = cliente.Id,
            Nombre = cliente.Nombre,
            Apellido = cliente.Apellido,
            Email = cliente.Email,
            FechaNacimiento = cliente.FechaNacimiento,
            CantidadReservas = cliente.Turnos.Count
        };
    }

}
