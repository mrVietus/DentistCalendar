using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IServiceRepository : IRepository
    {
        Task<ServiceDto> GetByIdAsync(int id);
        Task<ServiceAddResponse> CreateAsync(ServiceDto service);
        Task<bool> AddAssignedDentistsAsync(IEnumerable<int> dentistsIds, int serviceId);
        Task<bool> EditAssignedDentistsAsync(IEnumerable<int> dentistsIds, int serviceId);
        Task<bool> UpdateAsync(ServiceDto service);
        Task<bool> RemoveAsync(int id);
    }
}