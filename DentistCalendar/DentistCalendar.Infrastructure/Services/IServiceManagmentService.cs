using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services
{
    public interface IServiceManagmentService : IService
    {
        Task<ServiceDto> GetServiceByIdAsync(int Id);
        Task<IEnumerable<DentistOfficeDto>> GetServicesAsync(string profileId);
        Task<IEnumerable<DentistDto>> GetDentistsForServiceAsync(int dentistOfficeId);
        Task<object> TryAddServiceAsync(int dentistOfficeId, ServiceDto service,
            IEnumerable<int> assignedDentists, string successLink);
        Task<object> TryEditServiceAsync(ServiceDto service, IEnumerable<int> assignedDentists,
            string successLink);
        Task<bool> TryRemoveServiceAsync(int serviceId, int dentistOfficeId);
    }
}
