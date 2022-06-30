using DentistCalendar.Common.Enums;
using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services
{
    public interface IMenuElementService : IService
    {
        Task<bool> TryInitializeMenuElementsAsync();
        Task<IEnumerable<MenuElementDto>> GetMenuElementsForCurrentUserAsync(string usersAccountType);
        Task<IEnumerable<MenuElementDto>> GetMenuElementsForAnonymousUserAsync();
    }
}