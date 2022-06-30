using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services
{
    public interface ILicenseOwnerService : IService
    {
        Task<bool> AddDentistOfficeAsync(DentistOfficeDto dentistOffice, string profileId);
        Task<bool> EditDentistOfficeAsync(DentistOfficeDto dentistOffice);
        Task<bool> RemoveDentistOfficeAsync(int dentistOfficeId);
        Task<IEnumerable<DentistOfficeDto>> GetDentistOfficesAsync(string profileId);
        Task<DentistOfficeDto> GetDentistOfficeByIdAsync(int dentistOfficeId);
    }
}
