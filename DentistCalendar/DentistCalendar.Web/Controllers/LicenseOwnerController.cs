using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Services;
using DentistCalendar.Web.Models.LicenseOwner;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DentistCalendar.Web.Controllers
{
    [Authorize]
    public class LicenseOwnerController : Controller
    {
        private readonly ILicenseOwnerService _licenseOwnerService;

        public LicenseOwnerController(ILicenseOwnerService licenseOwnerService)
        {
            _licenseOwnerService = licenseOwnerService;
        }

        [Authorize(Policy = "LicenseOwner")]
        public IActionResult Index()
        {
            var userProfileId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrEmpty(userProfileId))
            {
                return RedirectToAction("CreateLicenseOwnersProfile", "Profile");
            }

            return View();
        }

        [Authorize(Policy = "LicenseOwner")]
        public async Task<IActionResult> DentistOfficeManagment()
        {
            var userProfileId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            if (string.IsNullOrEmpty(userProfileId))
            {
                return RedirectToAction("CreateLicenseOwnersProfile", "Profile");
            }

            var model = new DentistOfficeManagmentModel
            {
                DentistOffices = await _licenseOwnerService.GetDentistOfficesAsync(userProfileId)
            };

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<IActionResult> AddDentistOffice(DentistOfficeManagmentModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/LicenseOwner/DentistOfficeManagment.cshtml", model);
            }

            var userProfileId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var dentistOffice = new DentistOfficeDto
            {
                AboutUs = model.NewDentistOffice.AboutUs,
                Adress = $"{model.NewDentistOffice.City} {model.NewDentistOffice.Street} {model.NewDentistOffice.HouseNumber} {model.NewDentistOffice.ZipCode}",
                City = model.NewDentistOffice.City,
                Email = model.NewDentistOffice.Email,
                Name = model.NewDentistOffice.Name,
                Phone = model.NewDentistOffice.Phone
            };

            if (!await _licenseOwnerService.AddDentistOfficeAsync(dentistOffice, userProfileId))
            {
                ModelState.AddModelError("DentistOfficeWasNotAdded", $"Wystąpił nieoczekiwany błąd podczas dodawania pierwszego gabinetu dentystycznego.");
                return View("~/Views/LicenseOwner/DentistOfficeManagment.cshtml", model);
            }

            return RedirectToAction("DentistOfficeManagment", "LicenseOwner");
        }

        public ActionResult AddDentistOffice()
        {
            return PartialView("_AddDentistOfficeModal");
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<IActionResult> AddDentistOfficeModal(DentistOfficeManagmentModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = JsonConvert.SerializeObject(ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage));
                return Json(new { status = "error", modelErrors = errors });
            }

            var userProfileId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var apartamentNumber = GetApartamentNumber(model.NewDentistOffice.ApartamentNumber);

            var dentistOffice = new DentistOfficeDto
            {
                AboutUs = model.NewDentistOffice.AboutUs,
                Adress = $"{model.NewDentistOffice.City} {model.NewDentistOffice.Street} {model.NewDentistOffice.HouseNumber}{apartamentNumber} {model.NewDentistOffice.ZipCode}",
                City = model.NewDentistOffice.City,
                Email = model.NewDentistOffice.Email,
                Name = model.NewDentistOffice.Name,
                Phone = model.NewDentistOffice.Phone
            };

            if (!await _licenseOwnerService.AddDentistOfficeAsync(dentistOffice, userProfileId))
            {
                return Json(new { status = "error", modelErrors = "[\"Wystąpił nieoczekiwany błąd podczas dodawania gabinetu dentystycznego.\"]" });
            }

            return Json(new { status = "success", url = Url.Action("DentistOfficeManagment", "LicenseOwner") });
        }

        [Authorize(Policy = "LicenseOwner")]
        public async Task<ActionResult> EditDentistOffice(int id)
        {
            var dentistOfficeToEdit = await _licenseOwnerService.GetDentistOfficeByIdAsync(id);

            var model = CreateDentistOfficeModel(dentistOfficeToEdit);

            return PartialView("_EditDentistOffice", model);
        }

        [HttpPost]
        [Authorize(Policy = "LicenseOwner")]
        public async Task<JsonResult> EditDentistOffice(DentistOfficeModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = JsonConvert.SerializeObject(ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage));
                return Json(new { status = "error", modelErrors = errors });
            }

            var apartamentNumber = GetApartamentNumber(model.ApartamentNumber);

            var dentistOffice = new DentistOfficeDto
            {
                Id = model.Id,
                AboutUs = model.AboutUs,
                Adress = $"{model.City} {model.Street} {model.HouseNumber}{apartamentNumber} {model.ZipCode}",
                City = model.City,
                Email = model.Email,
                Name = model.Name,
                Phone = model.Phone
            };

            if (!await _licenseOwnerService.EditDentistOfficeAsync(dentistOffice))
            {
                return Json(new { status = "error", modelErrors = "[\"Wystąpił nieoczekiwany błąd podczas edycji gabinetu.\"]" });
            }

            return Json(new { status = "success", url = Url.Action("DentistOfficeManagment", "LicenseOwner") });
        }

        [Authorize(Policy = "LicenseOwner")]
        public async Task<JsonResult> RemoveDentistOffice(int id)
        {
            if (!await _licenseOwnerService.RemoveDentistOfficeAsync(id))
            {
                return Json(new { status = "error" });
            }

            return Json(new { status = "success", url = Url.Action("DentistOfficeManagment", "LicenseOwner") });
        }

        private string GetApartamentNumber(string apartamentNumber)
        {
            if (!string.IsNullOrEmpty(apartamentNumber))
            {
                apartamentNumber = $"/{apartamentNumber}";
            }

            return apartamentNumber;
        }

        private DentistOfficeModel CreateDentistOfficeModel(DentistOfficeDto dentistOfficeDto)
        {
            var trimmedAdress = dentistOfficeDto.Adress.Split(' ');

            var model = new DentistOfficeModel()
            {
                Id = dentistOfficeDto.Id,
                City = trimmedAdress[0],
                Street = trimmedAdress[1],
                ZipCode = trimmedAdress[3],
                AboutUs = dentistOfficeDto.AboutUs,
                Email = dentistOfficeDto.Email,
                Name = dentistOfficeDto.Name,
                Phone = dentistOfficeDto.Phone
            };

            if (trimmedAdress[2].Contains("/"))
            {
                var trimmedHouseAdress = trimmedAdress[2].Split('/');
                model.HouseNumber = trimmedHouseAdress[0];
                model.ApartamentNumber = trimmedHouseAdress[1];
            }
            else
            {
                model.HouseNumber = trimmedAdress[2];
            }

            return model;
        }
    }
}