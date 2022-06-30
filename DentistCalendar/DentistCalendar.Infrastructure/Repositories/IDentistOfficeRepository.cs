using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IDentistOfficeRepository : IRepository
    {
        Task<DentistOfficeDto> GetByIdAsync(int id);
        Task<DentistOfficeDto> GetWithServicesByIdAsync(int id);
        Task<IEnumerable<string>> GetDentistOficesCitiesAsync();
        Task<IEnumerable<DentistOfficeDto>> GetDentistOficesForCityAsync(string cityName);
        Task<bool> UpdateAsync(DentistOfficeDto dentistOffice);
        Task<bool> RemoveAsync(int dentistOfficeId);
        Task<DentistOfficeDto> GetByIWithRecepcionistAndDentistAsync(int id);
        Task<DentistOfficeDto> GetByIWithDentistsAsync(int id);
        Task<bool> CreateRecepcionistAsync(ReceptionistDto receptionist);
        Task<bool> RemoveRecepcionistAsync(ReceptionistDto receptionist);
        Task<bool> AddDentistAsync(int dentistId, int dentistOfficeId);
        Task<bool> RemoveDentistAsync(int dentistId, int dentistOfficeId);
        Task<bool> AddServiceAsync(int serviceId, int dentistOfficeId);
        Task<bool> RemoveServiceAsync(int serviceId, int dentistOfficeId);
    }
}