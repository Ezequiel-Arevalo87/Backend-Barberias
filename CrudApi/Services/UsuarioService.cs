using CrudApi.Data;
using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using Microsoft.EntityFrameworkCore;

public class UsuarioService : IUsuarioService
{
    private readonly ApplicationDbContext _context;

    public UsuarioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<UsuarioDTO>> GetUsuariosAsync()
    {
        return await _context.Usuarios
            .Select(u => new UsuarioDTO
            {
                Id = u.Id,
                NombreBarberia = u.NombreBarberia,
                Nombre = u.Nombre,
                Correo = u.Correo,
                Direccion = u.Direccion,
                Telefono = u.Telefono,
                Descripcion = u.Descripcion,
                FechaRegistro = u.FechaRegistro
            })
            .ToListAsync(); // Error: `ToListAsync()` no disponible para DTOs
    }


    public async Task<UsuarioDTO?> GetUsuarioByIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return null;

        return new UsuarioDTO { 
            Id = usuario.Id,
            NombreBarberia = usuario.NombreBarberia,
            Nombre = usuario.Nombre, 
            Correo = usuario.Correo, 
            Descripcion = usuario.Descripcion,
            FechaRegistro = usuario.FechaRegistro
        };
    }

    public async Task<Usuario> CreateUsuarioAsync(UsuarioCreateDTO usuarioDto)
    {
        // Verificar si el RoleId existe en la base de datos
        var roleExists = await _context.Roles.AnyAsync(r => r.Id == usuarioDto.RoleId);
        if (!roleExists)
        {
            throw new Exception("El RoleId proporcionado no existe en la base de datos.");
        }

        var usuario = new Usuario
        {
            NombreBarberia = usuarioDto.NombreBarberia,
            Nombre = usuarioDto.Nombre,
            Correo = usuarioDto.Correo,
            Descripcion = usuarioDto.Descripcion,
            Direccion = usuarioDto.Direccion,
            Telefono = usuarioDto.Telefono,
            Clave = usuarioDto.Clave,
            RoleId = usuarioDto.RoleId,
            FechaRegistro = usuarioDto.FechaRegistro
        };

        if (!string.IsNullOrWhiteSpace(usuarioDto.Clave))
        {
            usuario.Clave = PasswordHasher.HashPassword(usuarioDto.Clave);
        }

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return usuario;
    }

    public async Task<Usuario?> UpdateUsuarioAsync(int id, UsuarioUpdateDTO usuarioDto)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return null;

        // Actualizamos solo si se proporcionan nuevos datos
        usuario.Nombre = usuarioDto.Nombre ?? usuario.Nombre;
        usuario.Correo = usuarioDto.Correo ?? usuario.Correo;
        usuario.Descripcion = usuarioDto.Descripcion ?? usuario.Descripcion;
        usuario.Direccion = usuarioDto.Direccion ?? usuario.Direccion;
        usuario.Telefono = usuarioDto.Telefono ?? usuario.Telefono;

        // Si el usuario envía una nueva clave, encriptarla antes de actualizar
        if (!string.IsNullOrWhiteSpace(usuarioDto.Clave))
        {
            usuario.Clave = PasswordHasher.HashPassword(usuarioDto.Clave);
        }

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        return usuario;
    }

    public async Task<bool> DeleteUsuarioAsync(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null) return false;

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return true;
    }

    // ✅ Implementación del método faltante
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
