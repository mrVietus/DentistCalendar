using DentistCalendar.Common.Enums;
using DentistCalendar.Dto.DTO.Domain;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services
{
    public interface IProfileService : IService
    {
        Task<IProfile> GetProfileForUser(string userProfileId, string accountType);
        Task<IProfile> GetProfileForUser(string userProfileId, AccountType accountType);
        Task<string> TryCreateProfileAsync(IProfile profile, IFormFile avatar);
        Task<bool> TryUpdateProfileAsync(IProfile profile, IFormFile avatar);
    }
}