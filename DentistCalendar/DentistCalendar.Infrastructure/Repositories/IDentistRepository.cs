using DentistCalendar.Dto.DTO.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IDentistRepository : IRepository
    {
        Task<DentistDto> GetByIdAsync(int id);
        Task<DentistDto> GetByProfileIdAsync(Guid id);
        Task<IEnumerable<DentistDto>> GetWithServiceAndDentistOfficeAsync(int serviceId, int dentistOfficeId);
        Task<bool> CreateAsync(DentistDto dentist);
        Task<bool> UpdateAsync(DentistDto dentist);
        Task<bool> RemoveAsync(DentistDto dentist);
    }
}