using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentistCalendar.Web.Controllers
{
    [Authorize]
    public class ReceptionistController : Controller
    {
        [Authorize(Policy = "Receptionist")]
        public IActionResult Index()
        {
            return View();
        }
    }
}