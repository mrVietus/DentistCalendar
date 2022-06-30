using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CarCalendar.Web.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly string componentView = "~/Views/Shared/Components/Footer/Footer.cshtml";

        public async Task<IViewComponentResult> InvokeAsync()
        {
            int profileLinks = 2;
            return View(componentView, profileLinks);
        }
    }
}