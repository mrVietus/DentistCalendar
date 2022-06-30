using DentistCalendar.Infrastructure.Services;
using DentistCalendar.Web.Models.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DentistCalendar.Web.Controllers
{
    public class AppointmentController : Controller
    {
        IAppointmentService _appointmentService;

        public AppointmentController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [Authorize(Policy = "Patient")]
        public async Task<IActionResult> MyAppointments()
        {
            var userProfileId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            var model = new MyAppointmentsModel
            {
                PatientAppoinments = await _appointmentService.GetAppointmentsForPatient(userProfileId)
            };

            return View("MyAppointments", model);
        }

        [HttpGet]
        [Authorize(Policy = "Patient")]
        public async Task<JsonResult> GetAvaliableCities()
        {
            var data = await _appointmentService.GetAvaliableCities();
            return Json(data);
        }

        [HttpGet]
        [Authorize(Policy = "Patient")]
        public async Task<JsonResult> GetAvaliableOffices(string cityName)
        {
            var data = await _appointmentService.GetAvaliableOfficesAsync(cityName);
            return Json(data);
        }

        [HttpGet]
        [Authorize(Policy = "Patient")]
        public async Task<JsonResult> GetAvaliableServices(int dentistOfficeId)
        {
            var data = await _appointmentService.GetAvaliableServicesAsync(dentistOfficeId);
            return Json(data);
        }

        [HttpGet]
        [Authorize(Policy = "Patient")]
        public async Task<JsonResult> GetAvaliableDentists(int serviceId, int dentistOfficeId)
        {
            var data = await _appointmentService.GetAvaliableDentistsAsync(serviceId, dentistOfficeId);
            return Json(data);
        }

        [HttpGet]
        [Authorize(Policy = "Patient")]
        public async Task<JsonResult> GetAvaliableServiceHours(int serviceId, int dentistId, int dentistOfficeId, string date)
        {
            var data = await _appointmentService.GetAvaliableServiceHoursAsync(serviceId, dentistId, dentistOfficeId, date);
            return Json(data);
        }

        [HttpPost]
        [Authorize(Policy = "Patient")]
        public async Task<JsonResult> ScheduleService(int dentistOfficeId, int serviceId, int dentistId,  string serviceTime)
        {
            var data = await _appointmentService.ScheduleServiceAsync(serviceId, dentistId, dentistOfficeId, serviceTime,
                HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault(),
                Url.Action("MyAppointments", "Appointment"));

            return Json(data);
        }
    }
}