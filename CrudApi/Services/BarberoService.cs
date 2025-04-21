using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using CrudApi.Data;
using CrudApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace CrudApi.Services
{
    public class BarberoService : IBarberoService
    {
        private readonly ApplicationDbContext _context;

        public BarberoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BarberoResponseDTO>> GetBarberoAsync()
        {
            return await _context.Barberos
                .Include(b => b.Usuario)
                .Include(b => b.Barberia).ThenInclude(barb => barb.Usuario)
                .Select(b => new BarberoResponseDTO
                {
                    Id = b.Id,
                    BarberiaId = b.BarberiaId,
                    Nombre = b.Usuario.Nombre,
                    FotoBarbero = b.FotoBarbero,
                    Estado = b.Estado,
                    TipoDocumentoId = b.TipoDocumentoId,
                    NumeroDocumento = b.NumeroDocumento,
                    Direccion = b.Usuario.Direccion,
                    Telefono = b.Usuario.Telefono,
                    Email = b.Usuario.Correo,
                    SucursalId = b.SucursalId,
                    NombreBarberia = b.Barberia.Usuario != null ? b.Barberia.Usuario.Nombre : string.Empty
                }).ToListAsync();
        }

        public async Task<int?> ObtenerBarberoIdDesdeUsuarioIdAsync(int usuarioId)
        {
            var barbero = await _context.Barberos.FirstOrDefaultAsync(b => b.UsuarioId == usuarioId);
            return barbero?.Id;
        }

        public async Task<BarberoResponseDTO?> GetBarberoByIdAsync(int id)
        {
            var barbero = await _context.Barberos
                .Include(b => b.Usuario)
                .Include(b => b.Barberia).ThenInclude(barb => barb.Usuario)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (barbero == null) return null;

            return new BarberoResponseDTO
            {
                Id = barbero.Id,
                BarberiaId = barbero.BarberiaId,
                Nombre = barbero.Usuario.Nombre,
                TipoDocumentoId = barbero.TipoDocumentoId,
                NumeroDocumento = barbero.NumeroDocumento,
                FotoBarbero = barbero.FotoBarbero,
                Estado = barbero.Estado,
                Direccion = barbero.Usuario.Direccion,
                Telefono = barbero.Usuario.Telefono,
                Email = barbero.Usuario.Correo,
                NombreBarberia = barbero.Barberia?.Usuario?.Nombre ?? string.Empty
            };
        }

        public async Task<List<BarberoResponseDTO>> GetBarberosPorBarberiaAsync(int barberiaId)
        {
            return await _context.Barberos
                .Where(b => b.BarberiaId == barberiaId)
                .Include(b => b.Usuario)
                .Include(b => b.Barberia).ThenInclude(bb => bb.Usuario)
                .Select(b => new BarberoResponseDTO
                {
                    Id = b.Id,
                    BarberiaId = b.BarberiaId,
                    Nombre = b.Usuario.Nombre,
                    FotoBarbero = b.FotoBarbero,
                    Estado = b.Estado,
                    TipoDocumentoId = b.TipoDocumentoId,
                    NumeroDocumento = b.NumeroDocumento,
                    Direccion = b.Usuario.Direccion,
                    Telefono = b.Usuario.Telefono,
                    SucursalId = b.SucursalId,
                    Email = b.Usuario.Correo,
                    NombreBarberia = b.Barberia.Usuario != null ? b.Barberia.Usuario.Nombre : string.Empty
                }).ToListAsync();
        }

        public async Task<BarberoResponseDTO> CreateBarberoAsync(BarberoCreateDTO barberoDto)
        {
            var barberia = await _context.Barberias
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(b => b.Id == barberoDto.BarberiaId);

            if (barberia == null)
                throw new Exception($"No se encontró una barbería con el ID {barberoDto.BarberiaId}");

            var tipoDocumento = await _context.TipoDocumento.FindAsync(barberoDto.TipoDocumento);
            if (tipoDocumento == null)
                throw new Exception($"No se encontró el tipo de documento con el ID {barberoDto.TipoDocumento}");

            var usuario = new Usuario
            {
                Nombre = barberoDto.Nombre,
                Correo = barberoDto.Email,
                Clave = PasswordHasher.HashPassword(barberoDto.Password),
                RoleId = barberoDto.RoleId,
                Direccion = barberoDto.Direccion,
                Telefono = barberoDto.Telefono,
                FechaRegistro = DateTime.Now
                
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var nuevoBarbero = new Barbero
            {
                UsuarioId = usuario.Id,
                TipoDocumentoId = barberoDto.TipoDocumento,
                NumeroDocumento = barberoDto.NumeroDocumento,
                Estado = barberoDto.Estado,
                FotoBarbero = barberoDto.FotoBarbero,
                BarberiaId = barberoDto.BarberiaId,
                SucursalId = barberoDto.SucursalId
            };

            _context.Barberos.Add(nuevoBarbero);
            await _context.SaveChangesAsync();

            return new BarberoResponseDTO
            {
                Id = nuevoBarbero.Id,
                BarberiaId = nuevoBarbero.BarberiaId,
                SucursalId = nuevoBarbero.SucursalId,
                Nombre = usuario.Nombre,
                TipoDocumentoId = nuevoBarbero.TipoDocumentoId,
                NumeroDocumento = nuevoBarbero.NumeroDocumento,
                Direccion = usuario.Direccion,
                Telefono = usuario.Telefono,
                Email = usuario.Correo,
                Estado = nuevoBarbero.Estado,
                FotoBarbero = nuevoBarbero.FotoBarbero,
                NombreBarberia = barberia.Usuario?.Nombre ?? string.Empty
            };
        }

        public async Task<Barbero?> UpdateBarberoAsync(int id, BarberoUpdateDTO barberoDto)
        {
            var barbero = await _context.Barberos
                .Include(b => b.Usuario)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (barbero == null) return null;

            barbero.Usuario.Nombre = barberoDto.Nombre ?? barbero.Usuario.Nombre;
            barbero.Usuario.Direccion = barberoDto.Direccion ?? barbero.Usuario.Direccion;
            barbero.Usuario.Telefono = barberoDto.Telefono ?? barbero.Usuario.Telefono;
            barbero.Estado = barberoDto.Estado ?? barbero.Estado;
            barbero.FotoBarbero = barberoDto.FotoBarbero ?? barbero.FotoBarbero;

            await _context.SaveChangesAsync();
            return barbero;
        }

        public async Task<bool> RegistrarTokenFirebaseAsync(int barberoId, string nuevoToken)
        {
            var barbero = await _context.Barberos.FindAsync(barberoId);
            if (barbero == null) return false;

            // 1. Verifica si ya está asignado a un cliente
            var existeEnCliente = await _context.Clientes.AnyAsync(c => c.NotificationToken == nuevoToken);
            if (existeEnCliente)
            {
                Console.WriteLine("⚠️ El token ya está en un cliente. No se guarda en barbero.");
                return false;
            }

            // 2. Limpia en otros barberos
            var otrosConMismoToken = _context.Barberos
                .Where(b => b.NotificationToken == nuevoToken && b.Id != barberoId);

            await foreach (var otro in otrosConMismoToken.AsAsyncEnumerable())
            {
                otro.NotificationToken = null;
            }

            // 3. Asigna el token al barbero actual
            barbero.NotificationToken = nuevoToken;
            await _context.SaveChangesAsync();

            Console.WriteLine("✅ Token asignado al barbero correctamente.");
            return true;
        }


        public async Task<List<BarberoDTO>> ObtenerBarberosPorSucursalAsync(int sucursalId)
        {
            return await _context.Barberos
                .Where(b => b.SucursalId == sucursalId)
                .Include(b => b.Usuario)
                .Select(b => new BarberoDTO
                {
                    Id = b.Id,
                    BarberiaId = b.BarberiaId,
                    Nombre = b.Usuario.Nombre,
                    FotoBarbero = b.FotoBarbero,
                    Estado = b.Estado,
                    TipoDocumento = b.TipoDocumentoId,
                    NumeroDocumento = b.NumeroDocumento,
                    Direccion = b.Usuario.Direccion,
                    Telefono = b.Usuario.Telefono,
                    SucursalId = b.SucursalId,
                    Email = b.Usuario.Correo,
                    NombreBarberia = b.Barberia.Usuario != null ? b.Barberia.Usuario.Nombre : string.Empty
                }).ToListAsync();
        }

        public async Task<bool> RegistrarTokenFirebasePorUsuarioAsync(int usuarioId, string nuevoToken)
        {
            var barbero = await _context.Barberos
                .FirstOrDefaultAsync(b => b.UsuarioId == usuarioId);

            if (barbero == null)
                return false;

            // Limpia token duplicado en otros barberos
            var otrosConMismoToken = _context.Barberos
                .Where(b => b.NotificationToken == nuevoToken && b.UsuarioId != usuarioId);

            await foreach (var otro in otrosConMismoToken.AsAsyncEnumerable())
            {
                otro.NotificationToken = null;
            }

            barbero.NotificationToken = nuevoToken;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
