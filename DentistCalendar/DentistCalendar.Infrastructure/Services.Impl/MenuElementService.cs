using DentistCalendar.Common.Enums;
using DentistCalendar.Common.Logger;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.MainMenu;
using DentistCalendar.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services.Impl
{
    public class MenuElementService : IMenuElementService
    {
        private readonly ILoggerService _loggerService;
        private readonly IMenuElementRepository _menuElementRepository;

        public MenuElementService(ILoggerService loggerService, IMenuElementRepository menuElementRepository)
        {
            _loggerService = loggerService;
            _menuElementRepository = menuElementRepository;
        }

        public async Task<IEnumerable<MenuElementDto>> GetMenuElementsForAnonymousUserAsync()
        {
            return await _menuElementRepository.GetMenuElementsForAccountTypeAsync(AccountType.Everyone);
        }

        public async Task<IEnumerable<MenuElementDto>> GetMenuElementsForCurrentUserAsync(string usersAccountType)
        {
            var menuForCurrentUser = GetMenuElementsForAnonymousUserAsync().Result.ToList();
            var accountTypeMenuItems = await _menuElementRepository.GetMenuElementsForAccountTypeAsync(Enum.Parse<AccountType>(usersAccountType));

            menuForCurrentUser.AddRange(accountTypeMenuItems);

            return menuForCurrentUser;
        }

        public async Task<bool> TryInitializeMenuElementsAsync()
        {
            _loggerService.Info("Initialization of menu elements started.");

            var mainMenuInitializer = new MainMenuObjectInitialization();
            var mainMenu = mainMenuInitializer.GetMenuElements();
            var mainMenuFromRepo = await _menuElementRepository.GetMenuElementsAsync();

            _loggerService.Info("All menu elements objects ware initialized succesfuly.");

            foreach (var menuElement in mainMenu)
            {
                var menuElementFromRepo = mainMenuFromRepo?.FirstOrDefault(x => x.MenuElementId == menuElement.MenuElementId);
                if (menuElementFromRepo == null)
                {
                    _loggerService.Info($"Menu element with MenuElementId: {menuElement.MenuElementId} and display name {menuElement.DisplayName} is going to be created.");

                    if (await _menuElementRepository.CreateAsync(menuElement))
                    {
                        _loggerService.Info($"Menu element with MenuElementId: {menuElement.MenuElementId} and display name {menuElement.DisplayName} was created succesfuly.");
                    }
                    else
                    {
                        _loggerService.Error($"Creating of menu element with MenuElementId:{menuElement.MenuElementId} and name: {menuElement.DisplayName} failed. CHECK LOGS MENU IS WRONG!!");
                    }

                    continue;
                }

                if (menuElement.IsSame(menuElementFromRepo))
                {
                    _loggerService.Info($"Menu element with MenuElementId: {menuElement.MenuElementId} and display name {menuElement.DisplayName} wasn't change.");
                    continue;
                }

                _loggerService.Info($"Menu element with MenuElementId: {menuElement.MenuElementId} and display name {menuElement.DisplayName} is going to be updated.");
                if (!await _menuElementRepository.UpdateAsync(menuElement, menuElementFromRepo.Id))
                {
                    _loggerService.Error($"Updating of menu element with MenuElementId:{menuElement.MenuElementId} and name: {menuElement.DisplayName} failed. CHECK LOGS MENU IS WRONG!!");
                    return false;
                }
                _loggerService.Info($"Menu element with MenuElementId: {menuElement.MenuElementId} and display name {menuElement.DisplayName} was updated succesfuly.");
            }

            mainMenuFromRepo = await _menuElementRepository.GetMenuElementsAsync();

            if (mainMenu.Count() != mainMenuFromRepo.Count())
            {
                foreach (var repoMenuItem in mainMenuFromRepo)
                {
                    var menuElement = mainMenu.FirstOrDefault(x => x.MenuElementId == repoMenuItem.MenuElementId);
                    if (menuElement == null)
                    {
                        if (!await _menuElementRepository.RemoveAsync(menuElement.Id))
                        {
                            _loggerService.Error($"Remove of menu element with MenuElementId:{menuElement.MenuElementId} and name: {menuElement.DisplayName} failed. CHECK LOGS MENU IS WRONG!!");
                            return false;
                        }
                        _loggerService.Info($"Menu element with MenuElementId: {menuElement.MenuElementId} and display name {menuElement.DisplayName} was removed succesfuly.");
                    }
                }
            }

            return true;
        }
    }
}