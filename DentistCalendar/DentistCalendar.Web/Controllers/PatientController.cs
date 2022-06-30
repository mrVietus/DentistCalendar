using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentistCalendar.Web.Controllers
{
    [Authorize]
    public class PatientController : Controller
    {
        [Authorize(Policy = "Patient")]
        public IActionResult Index()
        {
            return View();
        }
    }
}