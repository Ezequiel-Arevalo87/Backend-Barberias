using CrudApi.DTOs;
using CrudApi.Models;

namespace CrudApi.Interfaces
{
    public interface IServiceService
    {
        Task<List<ServiceDTO>> GetServicesAsync();
        Task<ServiceDTO?> GetServiceByIdAsync(int id);
        Task<List<ServiceResponseDTO>> GetServicesByBarberAsync(int barberoId);
        Task<Service> CreateServiceAsync(ServiceDTO serviceDto);
        Task<Service?> UpdateServiceAsync(int id, ServiceDTO serviceDto);
        Task<List<ServiceDTO>> GetServiciosPorBarberoAsync(int barberoId);
        Task<bool> DeleteServiceAsync(int id);
    }
}
