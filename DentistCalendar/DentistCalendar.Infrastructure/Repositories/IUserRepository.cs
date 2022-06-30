using DentistCalendar.Dto.DTO.Domain;
using System;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IUserRepository : IRepository
    {
        Task<UserDto> GetByEmail(string email);
        Task<UserDto> GetByProfileIdAsync(Guid profileId);
        Task<bool> AddAsync(UserDto user);
        Task<bool> AddProfileId(string email, Guid profileId);
        Task<bool> UpdateAsync(string email, UserDto user);       
        Task<bool> UpdatePasswordAsync(string email, string newPassword, string newSalt);
        Task<bool> RemoveAsync(string email);
        Task<bool> ActivateUserAsync(string email);
    }
}