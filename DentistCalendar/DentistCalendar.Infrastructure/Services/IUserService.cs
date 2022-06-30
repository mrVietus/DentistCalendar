using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Dto.DTO.Domain;
using System;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services
{
    public interface IUserService : IService
    {
        Task<AuthenticationDto> AuthenticateAsync(string email, string password);
        Task<bool> TryRegisterAsync(RegistrationDto registrationDto);
        Task<bool> ConfirmEmailAsync(string registrationGuid, string email);
        Task<bool> UnregisterAsync(string email, string password, bool force);
        Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
        Task<UserDto> GetUserDataByEmailAsync(string email);
        Task<UserDto> GetUserDataByProfileIdAsync(Guid profileId);
    }
}