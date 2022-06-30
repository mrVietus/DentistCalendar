using DentistCalendar.Dto.DTO.Domain;
using System;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IPatientRepository : IRepository
    {
        Task<PatientDto> GetByIdAsync(int id);
        Task<PatientDto> GetByProfileIdAsync(Guid id);
        Task<bool> CreateAsync(PatientDto patient);
        Task<bool> UpdateAsync(PatientDto patient);
        Task<bool> RemoveAsync(PatientDto patient);
    }
}