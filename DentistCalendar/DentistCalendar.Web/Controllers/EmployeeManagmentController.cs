using DentistCalendar.Infrastructure.Services;
using DentistCalendar.Web.Models.EmployeeManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DentistCalendar.Web.Controllers
{
    [Authorize]
    public class EmployeeManagmentController : Controller
    {
        private readonly IEmployeeManagmentService _employeeManagmentService;

        public EmployeeManagmentController(IEmployeeManagmentService employeeManagmentService)
        {
            _employeeManagmentService = employeeManagmentService;
        }

        [Authorize(Policy = "LicenseOwner")]
        public async Task<IActionResult> Index()
        {
            var userProfileId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrEmpty(userProfileId))
            {
                return RedirectToAction("CreateLicenseOwnersProfile", "Profile");
            }

            var model = new EmployeeManagmentModel()
            {
                DentistOffices = await _employeeManagmentService.GetEmployeesAsync(userProfileId)
            };

            return View(model);
        }

        [Authorize(Policy = "LicenseOwner")]
        public IActionResult InviteDentist(int dentistOfficeId)
        {
            var model = new DentistRegistrationModel
            {
                DentistOfficeId = dentistOfficeId
            };

            return PartialView("_InviteDentist", model);
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<JsonResult> InviteDentist(DentistRegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = JsonConvert.SerializeObject(ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage));
                return Json(new { status = "error", modelErrors = errors });
            }

            var data = await _employeeManagmentService.TryInviteDentistAsync(
                model.Email,
                HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault(),
                model.DentistOfficeId,
                Url.Action("AcceptInvitationFromLink", "Invitation", new { invitationId = "HereShouldBeGuid", email = model.Email }, protocol: HttpContext.Request.Scheme),
                Url.Action("Index", "EmployeeManagment"));

            return Json(data);
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<JsonResult> RemoveDentistFromDentistOffice(int dentistId, int dentistOfficeId)
        {
            if (!await _employeeManagmentService.RemoveDentistAsync(dentistId, dentistOfficeId))
            {
                return Json(new { status = "error" });
            }

            return Json(new { status = "success", url = Url.Action("Index", "EmployeeManagment") });
        }

        [Authorize(Policy = "LicenseOwner")]
        public IActionResult AddRecepcionist(int dentistOfficeId)
        {
            var model = new RecepcionistRegistrationModel
            {
                DentistOfficeId = dentistOfficeId
            };

            return PartialView("_AddRecepcionist", model);
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<JsonResult> AddRecepcionist(RecepcionistRegistrationModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = JsonConvert.SerializeObject(ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage));
                return Json(new { status = "error", modelErrors = errors });
            }

            var data = await _employeeManagmentService.TryCreateRecepcionistAccountWithProfile(
                model.Email,
                model.TemporaryPassword,
                model.DentistOfficeId,
                Url.Action("ConfirmEmailAsync", "Authorization", new { registrationGuid = "HereShouldBeGuid", email = "HereShouldBeEmail" }, protocol: HttpContext.Request.Scheme),
                Url.Action("Index", "EmployeeManagment"));

            return Json(data);
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<JsonResult> RemoveRecepcionist(int id)
        {
            if (!await _employeeManagmentService.RemoveRecepcionistAsync(id))
            {
                return Json(new { status = "error" });
            }

            return Json(new { status = "success", url = Url.Action("Index", "EmployeeManagment") });
        }
    }
}