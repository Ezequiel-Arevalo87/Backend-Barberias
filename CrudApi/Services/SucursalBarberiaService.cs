using CrudApi.Data;
using Microsoft.EntityFrameworkCore;


public class SucursalBarberiaService : ISucursalBarberiaService
{
    private readonly ApplicationDbContext _context;

    public SucursalBarberiaService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SucursalBarberiaDTO>> GetAllAsync()
    {
        return await _context.SucursalesBarberia
            .Select(s => new SucursalBarberiaDTO
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Direccion = s.Direccion,
                Telefono = s.Telefono,
                FotoSucursal = s.FotoSucursal,
                BarberiaId = s.BarberiaId,
                TipoDocumentoId = s.TipoDocumentoId,
                NumeroDocumento = s.NumeroDocumento,
                Estado = s.Estado
            }).ToListAsync();
    }

    public async Task<SucursalBarberiaDTO?> GetByIdAsync(int id)
    {
        var sucursal = await _context.SucursalesBarberia.FindAsync(id);
        if (sucursal == null) return null;

        return new SucursalBarberiaDTO
        {
            Id = sucursal.Id,
            Nombre = sucursal.Nombre,
            Direccion = sucursal.Direccion,
            Telefono = sucursal.Telefono,
            FotoSucursal = sucursal.FotoSucursal,
            BarberiaId = sucursal.BarberiaId,
            TipoDocumentoId = sucursal.TipoDocumentoId,
            NumeroDocumento = sucursal.NumeroDocumento,
            Estado = sucursal.Estado
        };
    }

    public async Task<SucursalBarberiaDTO> CreateAsync(SucursalBarberiaDTO dto)
    {
        var nueva = new SucursalBarberia
        {
            Nombre = dto.Nombre,
            Direccion = dto.Direccion,
            Telefono = dto.Telefono,
            FotoSucursal = dto.FotoSucursal,
            BarberiaId = dto.BarberiaId,
            TipoDocumentoId = dto.TipoDocumentoId,
            NumeroDocumento = dto.NumeroDocumento,
            Estado = dto.Estado
        };

        _context.SucursalesBarberia.Add(nueva);
        await _context.SaveChangesAsync();

        dto.Id = nueva.Id;
        return dto;
    }

    public async Task<SucursalBarberiaDTO?> UpdateAsync(int id, SucursalBarberiaDTO dto)
    {
        var sucursal = await _context.SucursalesBarberia.FindAsync(id);
        if (sucursal == null) return null;

        sucursal.Nombre = dto.Nombre;
        sucursal.Direccion = dto.Direccion;
        sucursal.Telefono = dto.Telefono;
        sucursal.FotoSucursal = dto.FotoSucursal;
        sucursal.TipoDocumentoId = dto.TipoDocumentoId;
        sucursal.NumeroDocumento = dto.NumeroDocumento;
        sucursal.Estado = dto.Estado;

        await _context.SaveChangesAsync();
        return dto;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var sucursal = await _context.SucursalesBarberia.FindAsync(id);
        if (sucursal == null) return false;

        _context.SucursalesBarberia.Remove(sucursal);
        await _context.SaveChangesAsync();
        return true;
    }
}
