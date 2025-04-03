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
                .Include(b => b.Barberia)
                .Select(b => new BarberoResponseDTO
                {
                    Id = b.Id,
                    BarberiaId = b.BarberiaId,
                    Nombre = b.Nombre,
                    TipoDocumentoId = b.TipoDocumentoId,
                    NumeroDocumento = b.NumeroDocumento,
                    Direccion = b.Direccion,
                    Telefono = b.Telefono,
                    Email = b.Email,
                    NombreBarberia = b.Barberia.Nombre
                }).ToListAsync();
        }

        public async Task<BarberoResponseDTO?> GetBarberoByIdAsync(int id)
        {
            var barbero = await _context.Barberos
                .Include(b => b.Barberia)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (barbero == null) return null;

            return new BarberoResponseDTO
            {
                Id = barbero.Id,
                BarberiaId = barbero.BarberiaId,
                Nombre = barbero.Nombre,
                TipoDocumentoId = barbero.TipoDocumentoId,
                NumeroDocumento = barbero.NumeroDocumento,
                Direccion = barbero.Direccion,
                Telefono = barbero.Telefono,
                Email = barbero.Email,
                NombreBarberia = barbero.Barberia?.Nombre
            };
        }

        public async Task<List<BarberoResponseDTO>> GetBarberosPorBarberiaAsync(int barberiaId)
        {
            return await _context.Barberos
                .Where(b => b.BarberiaId == barberiaId)
                .Include(b => b.Barberia)
                .Select(b => new BarberoResponseDTO
                {
                    Id = b.Id,
                    BarberiaId = b.BarberiaId,
                    Nombre = b.Nombre,
                    TipoDocumentoId = b.TipoDocumentoId,
                    NumeroDocumento = b.NumeroDocumento,
                    Direccion = b.Direccion,
                    Telefono = b.Telefono,
                    Email = b.Email,
                    NombreBarberia = b.Barberia.Nombre
                }).ToListAsync();
        }

        public async Task<BarberoResponseDTO> CreateBarberoAsync(BarberoCreateDTO barberoDto)
        {
            var barberia = await _context.Barberias.FindAsync(barberoDto.BarberiaId);
            if (barberia == null)
                throw new Exception($"No se encontró una barbería con el ID {barberoDto.BarberiaId}");

            var tipoDocumento = await _context.TipoDocumento.FindAsync(barberoDto.TipoDocumento);
            if (tipoDocumento == null)
                throw new Exception($"No se encontró el tipo de documento con el ID {barberoDto.TipoDocumento}");

            var nuevoBarbero = new Barbero
            {
                Nombre = barberoDto.Nombre,
                TipoDocumentoId = barberoDto.TipoDocumento,
                NumeroDocumento = barberoDto.NumeroDocumento,
                Direccion = barberoDto.Direccion,
                Telefono = barberoDto.Telefono,
                Email = barberoDto.Email,
                Password = PasswordHasher.HashPassword(barberoDto.Password),
                RoleId = barberoDto.RoleId,
                BarberiaId = barberoDto.BarberiaId
            };

            _context.Barberos.Add(nuevoBarbero);
            await _context.SaveChangesAsync();

            return new BarberoResponseDTO
            {
                Id = nuevoBarbero.Id,
                BarberiaId = nuevoBarbero.BarberiaId,
                Nombre = nuevoBarbero.Nombre,
                TipoDocumentoId = nuevoBarbero.TipoDocumentoId,
                NumeroDocumento = nuevoBarbero.NumeroDocumento,
                Direccion = nuevoBarbero.Direccion,
                Telefono = nuevoBarbero.Telefono,
                Email = nuevoBarbero.Email,
                NombreBarberia = barberia.Nombre
            };
        }
    }
}
