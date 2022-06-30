using DentistCalendar.Common.Enums;
using DentistCalendar.Core.Entities;
using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IMenuElementRepository : IRepository
    {
        Task<bool> CreateAsync(MenuElement menuElement);
        Task<ICollection<MenuElement>> GetMenuElementsAsync();
        Task<IEnumerable<MenuElementDto>> GetMenuElementsForAccountTypeAsync(AccountType accountType);
        Task<bool> UpdateAsync(MenuElement menuElement, int menuElementId);
        Task<bool> RemoveAsync(int menuElementId);
    }
}