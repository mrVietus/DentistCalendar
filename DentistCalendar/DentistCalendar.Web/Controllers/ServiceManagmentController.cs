using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Services;
using DentistCalendar.Web.Models.ServiceManagment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DentistCalendar.Web.Controllers
{
    [Authorize]
    public class ServiceManagmentController : Controller
    {
        private readonly IServiceManagmentService _serviceManagmentService;

        public ServiceManagmentController(IServiceManagmentService serviceManagmentService)
        {
            _serviceManagmentService = serviceManagmentService;
        }

        public async Task<IActionResult> Index()
        {
            var userProfileId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrEmpty(userProfileId))
            {
                return RedirectToAction("CreateLicenseOwnersProfile", "Profile");
            }

            var model = new ServiceManagmentModel()
            {
                DentistOffices = await _serviceManagmentService.GetServicesAsync(userProfileId)
            };

            return View(model);
        }

        [Authorize(Policy = "LicenseOwner")]
        public async Task<IActionResult> AddService(int dentistOfficeId)
        {
            var model = new AddEditServiceModel
            {
                AvaliableDentists = CreateAvaliableDentistList(await _serviceManagmentService.GetDentistsForServiceAsync(dentistOfficeId)),
                DentistOfficeId = dentistOfficeId,
                AvaliableServiceTime = CreateAvaliableServiceTimeList()
            };

            return PartialView("_AddService", model);
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<JsonResult> AddService(AddEditServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = JsonConvert.SerializeObject(ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage));
                return Json(new { status = "error", modelErrors = errors });
            }

            var data = await _serviceManagmentService.TryAddServiceAsync(
                model.DentistOfficeId,
                model.Service,
                model.AssignedDentistListIds,
                Url.Action("Index", "ServiceManagment"));

            return Json(data);
        }

        [Authorize(Policy = "LicenseOwner")]
        public async Task<IActionResult> EditService(int Id, int DentistOfficeId)
        {
            var service = await _serviceManagmentService.GetServiceByIdAsync(Id);

            var model = new AddEditServiceModel
            {
                Service = service,
                AvaliableDentists = CreateAvaliableDentistList(await _serviceManagmentService.GetDentistsForServiceAsync(DentistOfficeId)),
                AvaliableServiceTime = CreateAvaliableServiceTimeList(),
                AssignedDentistListIds = service.Dentists.Any() ? service.Dentists.Select(x => x.Id) : Enumerable.Empty<int>()
            };

            return PartialView("_EditService", model);
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<JsonResult> EditService(AddEditServiceModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = JsonConvert.SerializeObject(ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage));
                return Json(new { status = "error", modelErrors = errors });
            }

            var data = await _serviceManagmentService.TryEditServiceAsync(
                model.Service,
                model.AssignedDentistListIds,
                Url.Action("Index", "ServiceManagment"));

            return Json(data);
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<JsonResult> RemoveService(int serviceId, int dentistOfficeId)
        {
            if (!await _serviceManagmentService.TryRemoveServiceAsync(serviceId, dentistOfficeId))
            {
                return Json(new { status = "error" });
            }

            return Json(new { status = "success", url = Url.Action("Index", "ServiceManagment") });
        }

        private static List<SelectListItem> CreateAvaliableServiceTimeList()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem {Text = "Piętnaście minut", Value = TimeSpan.FromMinutes(15).ToString()},
                    new SelectListItem {Text = "Pół godziny", Value = TimeSpan.FromMinutes(30).ToString()},
                    new SelectListItem {Text = "Czterdzieścipięć minut", Value = TimeSpan.FromMinutes(45).ToString()},
                    new SelectListItem {Text = "Godzina", Value = TimeSpan.FromHours(1).ToString()},
                    new SelectListItem {Text = "Półtorej godziny", Value = TimeSpan.FromMinutes(90).ToString()},
                    new SelectListItem {Text = "Dwie godziny", Value =  TimeSpan.FromHours(2).ToString()},
                    new SelectListItem {Text = "Trzy godziny", Value = TimeSpan.FromHours(3).ToString()},
                    new SelectListItem {Text = "Cztery godziny", Value = TimeSpan.FromHours(4).ToString()},
                    new SelectListItem {Text = "Pięć godzin", Value = TimeSpan.FromHours(5).ToString()}
                };
        }

        private static List<SelectListItem> CreateAvaliableDentistList(IEnumerable<DentistDto> dentistDtoList)
        {
            var selectListItemList = new List<SelectListItem>();

            foreach (var dentist in dentistDtoList)
            {
                selectListItemList.Add(new SelectListItem { Text = $"{dentist.Name} {dentist.LastName}", Value = dentist.Id.ToString() });
            }

            return selectListItemList;
        }
    }
}