using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using Microsoft.EntityFrameworkCore;

public class BarberiaService : IBarberiaService
{
    private readonly ApplicationDbContext _context;


    public BarberiaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BarberiaDTO>> GetBarberiaAsync()
    {
        return await _context.Barberias
            .Select(b => new BarberiaDTO
            {
                Id = b.Id,
                Nombre = b.Nombre,
                TipoDocumento = b.TipoDocumentoId,
                FotoBarberia = b.FotoBarberia,
                NumeroDocumento = b.NumeroDocumento,
                Email = b.Email,
                Direccion = b.Direccion,
                Telefono = b.Telefono,
          
            })
            .ToListAsync(); // Error: `ToListAsync()` no disponible para DTOs
    }


    public async Task<BarberiaDTO?> GetBarberiaByIdAsync(int id)
    {
        var barberia = await _context.Barberias.FindAsync(id);
        if (barberia == null) return null;

        return new BarberiaDTO
        {
            Id = barberia.Id,
            Nombre = barberia.Nombre,
            TipoDocumento = barberia.TipoDocumentoId,
            FotoBarberia = barberia.FotoBarberia,
            NumeroDocumento = barberia.NumeroDocumento,
            Email = barberia.Email,
            Direccion = barberia.Direccion,
            Telefono = barberia.Telefono,


        };
    }
 


    public async Task<Barberia> CreateBarberiaAsync(BarberiaCreateDTO barberiaDto)
    {
        // Verificar si el RoleId existe en la base de datos
        var roleExists = await _context.Roles.AnyAsync(r => r.Id == barberiaDto.RoleId);
        if (!roleExists)
        {
            throw new Exception("El RoleId proporcionado no existe en la base de datos.");
        }

        Console.WriteLine($"Recibido TipoDocumento ID: {barberiaDto.TipoDocumento}");

        // Buscar el TipoDocumento en la base de datos
        var tipoDocumento = await _context.TipoDocumento.FindAsync(barberiaDto.TipoDocumento);
        if (tipoDocumento == null)
        {
            throw new Exception("El TipoDocumento proporcionado no existe en la base de datos.");
        }

        var barberia = new Barberia
        {
            Nombre = barberiaDto.Nombre,
            Email = barberiaDto.Email,
            TipoDocumentoId = barberiaDto.TipoDocumento, // Guardamos el ID
            TipoDocumento = tipoDocumento, // Guardamos la entidad
            FotoBarberia = barberiaDto.FotoBarberia,
            NumeroDocumento = barberiaDto.NumeroDocumento,
            Direccion = barberiaDto.Direccion,
            Telefono = barberiaDto.Telefono,
            Password = PasswordHasher.HashPassword(barberiaDto.Password),
            RoleId = barberiaDto.RoleId
        };

        _context.Barberias.Add(barberia);
        await _context.SaveChangesAsync();
        return barberia;
    }

    // prueba 

    public async Task<Barberia?> UpdateBarberiaAsync(int id, BarberiaUpdateDTO barberiaDto)
    {
        var barberia = await _context.Barberias.FindAsync(id);
        if (barberia == null) return null;

        // Actualizamos solo si se proporcionan nuevos datos
        barberia.Nombre = barberiaDto.Nombre ?? barberia.Nombre;
        barberia.Direccion = barberiaDto.Direccion ?? barberia.Direccion;
        barberia.Telefono = barberiaDto.Telefono ?? barberia.Telefono;
        barberia.FotoBarberia = barberiaDto.FotoBarberia ?? barberia.FotoBarberia;

        // Si el barberia envía una nueva clave, encriptarla antes de actualizar
        //if (!string.IsNullOrWhiteSpace(barberiaDto.Clave))
        //{
        //    barberia.Clave = PasswordHasher.HashPassword(barberiaDto.Clave);
        //}

        _context.Barberias.Update(barberia);
        await _context.SaveChangesAsync();

        return barberia;
    }

    public async Task<bool> DeleteBarberiaAsync(int id)
    {
        var barberia = await _context.Barberias.FindAsync(id);
        if (barberia == null) return false;

        _context.Barberias.Remove(barberia);
        await _context.SaveChangesAsync();
        return true;
    }

    // ✅ Implementación del método faltante
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
