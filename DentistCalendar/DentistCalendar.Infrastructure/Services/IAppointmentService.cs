using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services
{
    public interface IAppointmentService : IService
    {
        Task<IEnumerable<string>> GetAvaliableCities();
        Task<IEnumerable<AppointmentDto>> GetAppointmentsForPatient(string userProfileId);
        Task<IEnumerable<DentistOfficeDto>> GetAvaliableOfficesAsync(string cityName);
        Task<IEnumerable<ServiceDto>> GetAvaliableServicesAsync(int dentistOfficeId);
        Task<IEnumerable<DentistDto>> GetAvaliableDentistsAsync(int serviceId, int dentistOfficeId);
        Task<IEnumerable<string>> GetAvaliableServiceHoursAsync(int serviceId, int dentistId, int dentistOfficeId, string date);
        Task<object> ScheduleServiceAsync(int serviceId, int dentistId, int dentistOfficeId, string serviceTime, string userProfileId, string successLink);
    }
}
