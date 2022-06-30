using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentistCalendar.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        [Authorize(Policy = "Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}