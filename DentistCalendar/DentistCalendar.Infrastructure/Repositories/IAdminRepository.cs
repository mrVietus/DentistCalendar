using DentistCalendar.Dto.DTO.Domain;
using System;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IAdminRepository : IRepository
    {
        Task<AdminDto> GetByIdAsync(int id);
        Task<AdminDto> GetByProfileIdAsync(Guid id);
        Task<AdminDto> GetByProfileIdWithDentistOfficesAsync(Guid id);
        Task<bool> CreateAsync(AdminDto admin);
        Task<bool> CreateDentistOfficeAsync(DentistOfficeDto dentistOffice);
        Task<bool> UpdateAsync(AdminDto admin);
        Task<bool> RemoveAsync(AdminDto admin);
    }
}