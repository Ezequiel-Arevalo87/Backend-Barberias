using CrudApi.Data;
using Microsoft.EntityFrameworkCore;
using CrudApi.DTOs;
using TuProyectoNamespace.Services;

public class ClienteService : IClienteService
{
    private readonly ApplicationDbContext _context;
    private readonly EmailService _emailService;

    public ClienteService(ApplicationDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }

    public async Task<ClienteDTO> RegistrarCliente(ClienteRegistroDTO clienteDto)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Correo == clienteDto.Email))
            throw new Exception("El email ya está registrado");

        var usuario = new Usuario
        {
            Nombre = clienteDto.Nombre,
            Correo = clienteDto.Email,
            Clave = PasswordHasher.HashPassword(clienteDto.Password),
            Direccion = clienteDto.Direccion,
            Telefono = clienteDto.Telefono,
            RoleId = clienteDto.RoleId,
            FechaRegistro = DateTime.Now
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        var cliente = new Cliente
        {
            UsuarioId = usuario.Id,
            Apellido = clienteDto.Apellido,
            FechaNacimiento = clienteDto.FechaNacimiento
        };

        _context.Clientes.Add(cliente);
        await _context.SaveChangesAsync();

        return new ClienteDTO
        {
            Id = cliente.Id,
            Nombre = usuario.Nombre,
            Apellido = cliente.Apellido,
            Email = usuario.Correo,
            FechaNacimiento = cliente.FechaNacimiento,
            CantidadReservas = 0
        };
    }

    public async Task<ClienteDTO?> ObtenerClientePorEmail(string email)
    {
        var cliente = await _context.Clientes
            .Include(c => c.Usuario)
            .Include(c => c.Turnos)
            .FirstOrDefaultAsync(c => c.Usuario.Correo == email);

        if (cliente == null)
            return null;

        return new ClienteDTO
        {
            Id = cliente.Id,
            Nombre = cliente.Usuario.Nombre,
            Apellido = cliente.Apellido,
            Email = cliente.Usuario.Correo,
            FechaNacimiento = cliente.FechaNacimiento,
            CantidadReservas = cliente.Turnos.Count
        };
    }

    public async Task<bool> RegistrarTokenFirebaseAsync(int clienteId, string nuevoToken)
    {
        var cliente = await _context.Clientes.FindAsync(clienteId);
        if (cliente == null) return false;

        var tokenEnBarbero = await _context.Barberos
            .AnyAsync(b => b.NotificationToken == nuevoToken);

        if (tokenEnBarbero)
        {
            Console.WriteLine("⚠️ Token ya está asignado a un barbero. No se guarda en cliente.");
            return false;
        }

        var otrosConMismoToken = _context.Clientes
            .Where(c => c.NotificationToken == nuevoToken && c.Id != clienteId);

        await foreach (var otro in otrosConMismoToken.AsAsyncEnumerable())
        {
            otro.NotificationToken = null;
        }

        cliente.NotificationToken = nuevoToken;
        await _context.SaveChangesAsync();

        Console.WriteLine("✅ Token asignado al cliente correctamente.");
        return true;
    }
}
