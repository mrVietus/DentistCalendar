using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarCalendar.Web.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly string componentView = "~/Views/Shared/Components/Header/Header.cshtml";

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int profileLinks = 2;
            return View(componentView, profileLinks);
        }
    }
}