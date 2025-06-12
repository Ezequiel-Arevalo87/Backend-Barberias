using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using CrudApi.Utils;
using Microsoft.EntityFrameworkCore;

public class BarberiaService : IBarberiaService
{
    private readonly ApplicationDbContext _context;

    public BarberiaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BarberiaResponseDTO>> GetBarberiasAsync(int usuarioId, int roleId)
    {
        Console.WriteLine($"UsuarioId: {usuarioId}, RoleId: {roleId}");

        var query = _context.Barberias
            .Include(b => b.Usuario)
            .Include(b => b.Sucursales)
            .AsQueryable();

        if (roleId != 1 && roleId != 3)
        {
            query = query.Where(b => b.UsuarioId == usuarioId);
        }

        var barberias = await query
            .Select(b => new BarberiaResponseDTO
            {
                Id = b.Id,
                Nombre = b.Usuario.Nombre,
                Correo = b.Usuario.Correo,
                Direccion = b.Usuario.Direccion,
                Telefono = b.Usuario.Telefono,
                FotoBarberia = b.FotoBarberia,
                NombreUsuario = b.Usuario.Nombre,
                Sucursales = b.Sucursales.Select(s => new SucursalBarberiaDTO
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Direccion = s.Direccion,
                    Telefono = s.Telefono,
                    BarberiaId = s.BarberiaId,
                    FotoSucursal = s.FotoSucursal,
                    Estado = s.Estado
                }).ToList()
            })
            .ToListAsync();

        return barberias;
    }



    public async Task<BarberiaResponseDTO?> GetBarberiaByIdAsync(int id)
    {
        var barberia = await _context.Barberias
            .Include(b => b.Usuario)
            .FirstOrDefaultAsync(b => b.UsuarioId == id);

        if (barberia == null) return null;

        return new BarberiaResponseDTO
        {
            Id = barberia.Id,
            Nombre = barberia.Usuario.Nombre,
            Correo = barberia.Usuario.Correo,
            FotoBarberia = barberia.FotoBarberia,
            Direccion = barberia.Usuario.Direccion,
            Telefono = barberia.Usuario.Telefono,
            NombreUsuario = barberia.Usuario.Nombre
        };
    }

    public async Task<BarberiaResponseDTO> CreateBarberiaAsync(BarberiaCreateDTO barberiaDto)
    {
        // Crear el usuario desde el objeto anidado
        var usuario = new Usuario
        {
            Nombre = barberiaDto.Usuario.Nombre,
            Correo = barberiaDto.Usuario.Correo,
            Clave = PasswordHasher.HashPassword(barberiaDto.Usuario.Clave),
            RoleId = barberiaDto.Usuario.RoleId,
            Direccion = barberiaDto.Usuario.Direccion,
            Telefono = barberiaDto.Usuario.Telefono,
            FechaRegistro = DateTime.Now.ToUniversalTime()
        };

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        // Crear la barbería con relación al usuario recién creado
        var nuevaBarberia = new Barberia
        {
            UsuarioId = usuario.Id,
            TipoDocumento = barberiaDto.TipoDocumento,
            NumeroDocumento = barberiaDto.NumeroDocumento,
            FotoBarberia = barberiaDto.FotoBarberia
        };

        _context.Barberias.Add(nuevaBarberia);
        await _context.SaveChangesAsync();

        return new BarberiaResponseDTO
        {
            Id = nuevaBarberia.Id,
            Nombre = usuario.Nombre,
            Correo = usuario.Correo,
            Direccion = usuario.Direccion,
            Telefono = usuario.Telefono,
            NombreUsuario = usuario.Nombre
        };
    }

    public async Task<Barberia?> UpdateBarberiaAsync(int id, BarberiaUpdateDTO barberiaDto)
    {
        var barberia = await _context.Barberias
            .Include(b => b.Usuario)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (barberia == null) return null;

        barberia.Usuario.Nombre = barberiaDto.Nombre ?? barberia.Usuario.Nombre;
        barberia.Usuario.Direccion = barberiaDto.Direccion ?? barberia.Usuario.Direccion;
        barberia.Usuario.Telefono = barberiaDto.Telefono ?? barberia.Usuario.Telefono;
        barberia.FotoBarberia = barberiaDto.FotoBarberia ?? barberia.FotoBarberia;

        await _context.SaveChangesAsync();
        return barberia;
    }

    public async Task<bool> DeleteBarberiaAsync(int id)
    {
        var barberia = await _context.Barberias
            .Include(b => b.Usuario)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (barberia == null) return false;

        _context.Usuarios.Remove(barberia.Usuario);
        _context.Barberias.Remove(barberia);
        await _context.SaveChangesAsync();

        return true;
    }
    public async Task<List<BarberiaResponseDTO>> ObtenerTodasAsync()
    {
        return await _context.Barberias
            .Where(b => b.Estado == 1)
            .Include(b => b.Usuario)
            .Include(b => b.Sucursales)
            .Select(b => new BarberiaResponseDTO
            {
                Id = b.Id,
                Nombre = b.Usuario.Nombre,
                Correo = b.Usuario.Correo,
                FotoBarberia = b.FotoBarberia,
                Direccion = b.Usuario.Direccion,
                Telefono = b.Usuario.Telefono,
                NombreUsuario = b.Usuario.Nombre,
                Sucursales = b.Sucursales.Select(s => new SucursalBarberiaDTO
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Direccion = s.Direccion,
                    Telefono = s.Telefono,
                    BarberiaId = s.BarberiaId,
                    FotoSucursal = s.FotoSucursal,
                    Estado = s.Estado
                }).ToList()
            }).ToListAsync();
    }

    public async Task<List<BarberiaResponseDTO>> ObtenerPorUsuarioId(int usuarioId)
    {
        return await _context.Barberias
            .Where(b => b.UsuarioId == usuarioId && b.Estado == 1)
            .Include(b => b.Usuario)
            .Include(b => b.Sucursales)
            .Select(b => new BarberiaResponseDTO
            {
                Id = b.Id,
                Nombre = b.Usuario.Nombre,
                Correo = b.Usuario.Correo,
                FotoBarberia = b.FotoBarberia,
                Direccion = b.Usuario.Direccion,
                Telefono = b.Usuario.Telefono,
                NombreUsuario = b.Usuario.Nombre,
                Sucursales = b.Sucursales.Select(s => new SucursalBarberiaDTO
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Direccion = s.Direccion,
                    Telefono = s.Telefono,
                    BarberiaId = s.BarberiaId,
                    FotoSucursal = s.FotoSucursal,
                    Estado = s.Estado
                }).ToList()
            }).ToListAsync();
    }


    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
