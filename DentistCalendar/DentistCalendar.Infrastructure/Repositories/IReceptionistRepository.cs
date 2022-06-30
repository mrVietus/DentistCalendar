using DentistCalendar.Dto.DTO.Domain;
using System;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IReceptionistRepository : IRepository
    {
        Task<ReceptionistDto> GetByIdAsync(int id);
        Task<ReceptionistDto> GetByProfileIdAsync(Guid id);
        Task<bool> UpdateAsync(ReceptionistDto receptionist);
        Task<bool> RemoveAsync(ReceptionistDto receptionist);
    }
}