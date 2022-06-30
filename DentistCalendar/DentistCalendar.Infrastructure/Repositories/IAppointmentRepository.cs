using DentistCalendar.Dto.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IAppointmentRepository : IRepository
    {
        Task<IEnumerable<AppointmentDto>> GetAllForDentistAsync(DentistDto dentist);
        Task<IEnumerable<AppointmentDto>> GetForDentistWithDateAsync(int dentistId, DateTime date);
        Task<IEnumerable<AppointmentDto>> GetAllForPatientAsync(PatientDto patient);
        Task<IEnumerable<AppointmentDto>> GetForPatientWithDate(PatientDto patient, DateTime fromDate, DateTime toDate);
        Task<IEnumerable<AppointmentDto>> GetAllForDentistOfficeAsync(DentistOfficeDto dentistOffice);
        Task<IEnumerable<AppointmentDto>> GetForDentistOfficeWithDateAsync(DentistOfficeDto dentistOffice, DateTime fromDate, DateTime toDate);
        Task<bool> CreateAsync(int patientId, int dentistId, int serviceId, DateTime appointmentDate);
        Task<bool> UpdateAsync(AppointmentDto appointment);
        Task<bool> RemoveAsync(int id);
    }
}