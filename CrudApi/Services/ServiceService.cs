using CrudApi.DTOs;
using CrudApi.Interfaces;
using CrudApi.Models;
using CrudApi.Data;
using Microsoft.EntityFrameworkCore;

namespace CrudApi.Services
{
    public class ServiceService : IServiceService
    {
        private readonly ApplicationDbContext _context;

        public ServiceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ServiceDTO>> GetServicesAsync()
        {
            return await _context.Servicios
                .Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    Servicio = s.Nombre,
                    Estado = s.Estado,
                    Descripcion = s.Descripcion,
                    Foto = s.Foto,
                    PrecioEspecial = s.PrecioEspecial,
                    Precio = s.Precio,
                    Tiempo = s.Tiempo,
                    Observacion = s.Observacion,
                    BarberoId = s.BarberoId
                }).ToListAsync();
        }

        public async Task<ServiceDTO?> GetServiceByIdAsync(int id)
        {
            var service = await _context.Servicios.FindAsync(id);
            return service == null ? null : new ServiceDTO
            {
                Id = service.Id,
                Servicio = service.Nombre,
                Estado = service.Estado,
                Descripcion = service.Descripcion,
                Foto = service.Foto,
                PrecioEspecial = service.PrecioEspecial,
                Precio = service.Precio,
                Tiempo = service.Tiempo,
                Observacion = service.Observacion,
                BarberoId = service.BarberoId
            };
        }

        public async Task<List<ServiceResponseDTO>> GetServicesByBarberAsync(int barberoId)
        {
            return await _context.Servicios
                .Include(s => s.Barbero)
                    .ThenInclude(b => b.Usuario)
                .Include(s => s.Barbero)
                    .ThenInclude(b => b.Barberia)
                        .ThenInclude(barberia => barberia.Usuario) // 👈 para el nombre de la barbería
                .Include(s => s.Barbero)
                    .ThenInclude(b => b.Sucursal)
                .Where(s => s.BarberoId == barberoId)
                .Select(s => new ServiceResponseDTO
                {
                    Id = s.Id,
                    Servicio = s.Nombre,
                    Estado = s.Estado,
                    Descripcion = s.Descripcion,
                    Foto = s.Foto,
                    PrecioEspecial = s.PrecioEspecial,
                    Precio = s.Precio,
                    Tiempo = s.Tiempo,
                    Observacion = s.Observacion,

                    BarberoId = s.BarberoId,
                    BarberoNombre = s.Barbero.Usuario.Nombre,
                    BarberoEmail = s.Barbero.Usuario.Correo,

                    BarberiaId = s.Barbero.BarberiaId,
                    BarberiaNombre = s.Barbero.Barberia.Usuario.Nombre,

                    SucursalId = s.Barbero.SucursalId,
                    SucursalNombre = s.Barbero.Sucursal != null ? s.Barbero.Sucursal.Nombre : null
                })
                .ToListAsync();
        }


        public async Task<List<ServiceDTO>> GetServiciosPorBarberoAsync(int barberoId)
        {
            return await _context.Servicios
                .Where(s => s.BarberoId == barberoId)
                .Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    Servicio = s.Nombre,
                    Precio = s.Precio,
                    PrecioEspecial = s.PrecioEspecial,
                    Estado = s.Estado,
                    Descripcion = s.Descripcion,
                    Foto = s.Foto,
                    Tiempo = s.Tiempo,
                    Observacion = s.Observacion,
                    BarberoId = s.BarberoId
                })
                .ToListAsync();
        }

        public async Task<Service> CreateServiceAsync(ServiceDTO serviceDto)
        {
            var nuevoServicio = new Service
            {
                Nombre = serviceDto.Servicio,
                Estado = serviceDto.Estado,
                Descripcion = serviceDto.Descripcion,
                Foto = serviceDto.Foto,
                PrecioEspecial = serviceDto.PrecioEspecial,
                Precio = serviceDto.Precio,
                Tiempo = serviceDto.Tiempo,
                Observacion = serviceDto.Observacion,
                BarberoId = serviceDto.BarberoId
            };

            _context.Servicios.Add(nuevoServicio);
            await _context.SaveChangesAsync();

            return nuevoServicio;
        }

        public async Task<Service?> UpdateServiceAsync(int id, ServiceDTO serviceDto)
        {
            var service = await _context.Servicios.FindAsync(id);
            if (service == null) return null;

            service.Nombre = serviceDto.Servicio;
            service.Estado = serviceDto.Estado;
            service.Descripcion = serviceDto.Descripcion;
            service.Foto = serviceDto.Foto;
            service.PrecioEspecial = serviceDto.PrecioEspecial;
            service.Precio = serviceDto.Precio;
            service.Tiempo = serviceDto.Tiempo;
            service.Observacion = serviceDto.Observacion;

            await _context.SaveChangesAsync();
            return service;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var service = await _context.Servicios.FindAsync(id);
            if (service == null) return false;

            _context.Servicios.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
