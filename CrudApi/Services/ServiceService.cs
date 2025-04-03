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

        // ✅ Obtener todos los servicios
        public async Task<List<ServiceDTO>> GetServicesAsync()
        {
            return await _context.Servicios
                .Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    Servicio = s.Servicio,
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

        // ✅ Obtener turnos por barbero
        public async Task<List<TurnoDTO>> GetTurnosPorBarberoAsync(int barberoId)
        {
            var turnos = await _context.Turnos
                .Where(t => t.BarberoId == barberoId)
                .Select(t => new TurnoDTO
                {
                    Id = t.TurnoId,
                    BarberoId = t.BarberoId,
                    ServicioId = t.ServicioId,
                    ClienteId = t.ClienteId,
                    Fecha = t.FechaHora,
                    FechaHoraInicio = t.FechaHoraInicio,
                    Duracion = t.Duracion,
                    Estado = t.Estado,
                    ClienteNombre = t.Cliente.Nombre,
                    ClienteApellido = t.Cliente.Apellido,
                    ClienteEmail = t.Cliente.Email,
                    ClienteFechaNacimiento = t.Cliente.FechaNacimiento,
                    ServicioNombre = t.Servicio.Servicio,
                    ServicioDescripcion = t.Servicio.Descripcion,
                    ServicioPrecio = t.Servicio.Precio,
                    ServicioPrecioEspecial = t.Servicio.PrecioEspecial
                }).ToListAsync();

            var horaActual = DateTime.Now;

            foreach (var turno in turnos)
            {
                DateTime fechaHoraFin = turno.FechaHoraInicio.Add(turno.Duracion);

                turno.Estado = horaActual < turno.FechaHoraInicio ? "Pendiente" :
                               horaActual <= fechaHoraFin ? "En Progreso" : "Completado";
            }

            return turnos;

          
        }

        // ✅ Obtener servicio por ID
        public async Task<ServiceDTO?> GetServiceByIdAsync(int id)
        {
            var service = await _context.Servicios.FindAsync(id);
            return service == null ? null : new ServiceDTO
            {
                Id = service.Id,
                Servicio = service.Servicio,
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

        // obtner servicio por barbero
        public async Task<List<ServiceResponseDTO>> GetServicesByBarberAsync(int barberoId)
        {
            var service = await _context.Servicios
                  .Where(s => s.BarberoId == barberoId)
                .Select(s => new ServiceResponseDTO

                {
                Id = s.Id,
                Servicio = s.Servicio,
                Estado = s.Estado,
                Descripcion = s.Descripcion,
                Foto = s.Foto,
                PrecioEspecial = s.PrecioEspecial,
                Precio = s.Precio,
                Tiempo = s.Tiempo,
                Observacion = s.Observacion,
                BarberoId = s.BarberoId
            }).ToListAsync();
            return service;
        }

        // ✅ Obtener servicios por barbero
        public async Task<List<ServiceDTO>> GetServicesPorBarberoAsync(int barberoId)
        {
            return await _context.Servicios
                .Where(s => s.BarberoId == barberoId)
                .Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    BarberoId = s.BarberoId,
                    Servicio = s.Servicio,
                    Estado = s.Estado,
                    Descripcion = s.Descripcion,
                    Foto = s.Foto,
                    PrecioEspecial = s.PrecioEspecial,
                    Tiempo = s.Tiempo,
                    Observacion = s.Observacion
                }).ToListAsync();
        }

        // ✅ Crear un nuevo servicio
        public async Task<Service> CreateServiceAsync(ServiceDTO serviceDto)
        {
            var nuevoServicio = new Service
            {
                Servicio = serviceDto.Servicio,
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

        // ✅ Actualizar un servicio existente
        public async Task<Service?> UpdateServiceAsync(int id, ServiceDTO serviceDto)
        {
            var service = await _context.Servicios.FindAsync(id);
            if (service == null) return null;

            service.Servicio = serviceDto.Servicio;
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

        // ✅ Cerrar turnos vencidos automáticamente
        public async Task CerrarTurnosVencidosAsync()
        {
            var ahora = DateTime.Now;

            var turnos = await _context.Turnos
                .Where(t => t.Estado == "Pendiente")
                .ToListAsync();

            foreach (var turno in turnos)
            {
                var horaInicio = turno.FechaHora;
                var horaFin = horaInicio.Add(turno.Duracion);

                turno.Estado = ahora >= horaFin ? "CERRADO" :
                               ahora >= horaInicio ? "EN_PROCESO" : turno.Estado;
            }

            await _context.SaveChangesAsync();
            Console.WriteLine(turnos.Any() ? "Estados de turnos actualizados automáticamente." : "No hay turnos pendientes para actualizar.");
        }

        // ✅ Eliminar un servicio
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
