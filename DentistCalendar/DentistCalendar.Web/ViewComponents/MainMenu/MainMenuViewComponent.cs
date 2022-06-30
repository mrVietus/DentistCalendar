using AutoMapper;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.MappedModels.MainMenu;
using DentistCalendar.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CarCalendar.Web.ViewComponents
{
    public class MainMenuViewComponent : ViewComponent
    {
        private readonly string componentView = "~/Views/Shared/Components/MainMenu/MainMenu.cshtml";

        private readonly IMapper _mapper;
        private readonly IMenuElementService _menuElementService;

        public MainMenuViewComponent(IMapper mapper, IMenuElementService menuElementService)
        {
            _mapper = mapper;
            _menuElementService = menuElementService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var role = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();
            if (role != null)
            {
                var roleMenuItems = await _menuElementService.GetMenuElementsForCurrentUserAsync(role);
                return View(componentView, _mapper.Map<IEnumerable<MenuElementDto>, IEnumerable<MainMenuModel>>(roleMenuItems));
            }
            var menuItems = await _menuElementService.GetMenuElementsForAnonymousUserAsync();

            return View(componentView, _mapper.Map<IEnumerable<MenuElementDto>, IEnumerable<MainMenuModel>>(menuItems));
        }
    }
}