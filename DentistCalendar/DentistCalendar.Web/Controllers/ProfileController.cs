using DentistCalendar.Common.Enums;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Services;
using DentistCalendar.Web.Models.Profile;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DentistCalendar.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        public async Task<IActionResult> Index(bool succesfulyUpdated = false)
        {
            var userProfileId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();
            var accountType = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).SingleOrDefault();

            var profile = await _profileService.GetProfileForUser(userProfileId, accountType);

            if (profile != null)
            {
                switch (profile)
                {
                    case AdminDto adminDto:
                        return AdminProfileView(adminDto, succesfulyUpdated);
                    case DentistDto dentistDto:
                        return DentistProfileView(dentistDto, succesfulyUpdated);
                    case PatientDto patientDto:
                        return PatientProfileView(patientDto, succesfulyUpdated);
                    case ReceptionistDto receptionistDto:
                        return RecepcionistProfileView(receptionistDto, succesfulyUpdated);
                }
            }

            return CreateProfile(accountType);
        }

        public IActionResult CreateLicenseOwnersProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateLicenseOwnersProfile(LicenseOwnerProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var adminDto = new AdminDto
            {
                Name = model.Name,
                LastName = model.LastName,
                Adress = $"{model.City} {model.Street} {model.HouseNumber} {model.ZipCode}",
                MobilePhone = model.MobilePhone,
                Email = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault(),
                NIP = model.NIP
            };

            var profileId = await _profileService.TryCreateProfileAsync(adminDto, HttpContext.Request.Form.Files.FirstOrDefault());

            if (!string.IsNullOrEmpty(profileId))
            {
                await AddProfileIdClaim(profileId);

                return RedirectToAction("Index", "LicenseOwner");
            }

            return RedirectToAction("CreateLicenseOwnersProfile", "Profile"); //Something went wrong register one more time
        }

        [HttpPost]
        public async Task<IActionResult> EditLicenseOwnersProfile(LicenseOwnerProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Profile/LicenseOwnerProfile.cshtml", model);
            }

            var adminDto = new AdminDto
            {
                ProfileId = Guid.Parse(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault()),
                Name = model.Name,
                LastName = model.LastName,
                Adress = $"{model.City} {model.Street} {model.HouseNumber} {model.ZipCode}",
                MobilePhone = model.MobilePhone,
                Email = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault(),
                NIP = model.NIP,
                ProfileImageUrl = model.ProfileImageUrl
            };

            if (!await _profileService.TryUpdateProfileAsync(adminDto, HttpContext.Request.Form.Files.FirstOrDefault()))
            {
                ModelState.AddModelError("ProfileWasNotUpdated", $"Wystąpił nieoczekiwany błąd podczas edycji twojego profilu.");
                return View("~/Views/Profile/LicenseOwnerProfile.cshtml", model);
            }

            return RedirectToAction("Index", "Profile", new { succesfulyUpdated = true });
        }

        public IActionResult CreateDentistsProfile()
        {
            return View("~/Views/Profile/CreateDentistsProfile.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreateDentistsProfile(DentistProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var dentistDto = new DentistDto
            {
                Name = model.Name,
                LastName = model.LastName,
                Adress = $"{model.City} {model.Street} {model.HouseNumber} {model.ZipCode}",
                MobilePhone = model.MobilePhone,
                Email = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault(),
                Description = model.Description,
                DoctorTitle = model.DoctorTitle,
                ProfileImageUrl = model.ProfileImageUrl
            };

            var profileId = await _profileService.TryCreateProfileAsync(dentistDto, HttpContext.Request.Form.Files.FirstOrDefault());

            if (!string.IsNullOrEmpty(profileId))
            {
                await AddProfileIdClaim(profileId);

                return RedirectToAction("Index", "Profile");
            }

            return RedirectToAction("CreateDentistsProfile", "Profile"); //Something went wrong register one more time
        }

        [HttpPost]
        public async Task<IActionResult> EditDentistsProfile(DentistProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Profile/DentistProfile.cshtml", model);
            }

            var dentistDto = new DentistDto
            {
                ProfileId = Guid.Parse(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault()),
                Name = model.Name,
                LastName = model.LastName,
                Adress = $"{model.City} {model.Street} {model.HouseNumber} {model.ZipCode}",
                MobilePhone = model.MobilePhone,
                Email = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault(),
                ProfileImageUrl = model.ProfileImageUrl,
                Description = model.Description,
                DoctorTitle = model.DoctorTitle
            };

            if (!await _profileService.TryUpdateProfileAsync(dentistDto, HttpContext.Request.Form.Files.FirstOrDefault()))
            {
                ModelState.AddModelError("ProfileWasNotUpdated", $"Wystąpił nieoczekiwany błąd podczas edycji twojego profilu.");
                return View("~/Views/Profile/DentistProfile.cshtml", model);
            }

            return RedirectToAction("Index", "Profile", new { succesfulyUpdated = true });
        }

        public IActionResult CreatePatientsProfile()
        {
            return View("~/Views/Profile/CreatePatientsProfile.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreatePatientsProfile(PatientProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var patientDto = new PatientDto
            {
                Name = model.Name,
                LastName = model.LastName,
                MobilePhone = model.MobilePhone,
                Email = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault(),
                ProfileImageUrl = model.ProfileImageUrl
            };

            var profileId = await _profileService.TryCreateProfileAsync(patientDto, HttpContext.Request.Form.Files.FirstOrDefault());

            if (!string.IsNullOrEmpty(profileId))
            {
                await AddProfileIdClaim(profileId);

                return RedirectToAction("Index", "Patient");
            }

            return RedirectToAction("CreatePatientsProfile", "Profile"); //Something went wrong register one more time
        }

        [HttpPost]
        public async Task<IActionResult> EditPatientsProfile(PatientProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Profile/PatientProfile.cshtml", model);
            }

            var patientDto = new PatientDto
            {
                ProfileId = Guid.Parse(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault()),
                Name = model.Name,
                LastName = model.LastName,
                MobilePhone = model.MobilePhone,
                Email = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault(),
                ProfileImageUrl = model.ProfileImageUrl
            };

            if (!await _profileService.TryUpdateProfileAsync(patientDto, HttpContext.Request.Form.Files.FirstOrDefault()))
            {
                ModelState.AddModelError("ProfileWasNotUpdated", $"Wystąpił nieoczekiwany błąd podczas edycji twojego profilu.");
                return View("~/Views/Profile/PatientProfile.cshtml", model);
            }

            return RedirectToAction("Index", "Profile", new { succesfulyUpdated = true });
        }

        [HttpPost]
        public async Task<IActionResult> EditRecepcionistProfile(RecepcionistProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Profile/RecepcionistProfile.cshtml", model);
            }

            var recepcionistDto = new ReceptionistDto
            {
                ProfileId = Guid.Parse(HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault()),
                Name = model.Name,
                LastName = model.LastName,
                MobilePhone = model.MobilePhone,
                Email = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault(),
                ProfileImageUrl = model.ProfileImageUrl
            };

            if (!await _profileService.TryUpdateProfileAsync(recepcionistDto, HttpContext.Request.Form.Files.FirstOrDefault()))
            {
                ModelState.AddModelError("ProfileWasNotUpdated", $"Wystąpił nieoczekiwany błąd podczas edycji twojego profilu.");
                return View("~/Views/Profile/RecepcionistProfile.cshtml", model);
            }

            return RedirectToAction("Index", "Profile", new { succesfulyUpdated = true });
        }

        #region PrivateMethods

        private IActionResult CreateProfile(string accountType)
        {
            switch ((AccountType)Enum.Parse(typeof(AccountType), accountType))
            {
                case AccountType.Admin:
                    return RedirectToAction("CreateLicenseOwnersProfile", "Profile");
                case AccountType.Dentist:
                    return RedirectToAction("CreateDentistsProfile", "Profile");
                case AccountType.Patient:
                    return RedirectToAction("CreatePatientsProfile", "Profile");
                default:
                    return RedirectToAction("accessdenied", "Authorization");
            }
        }

        private async Task AddProfileIdClaim(string profileId)
        {
            var claims = HttpContext.User.Claims.ToList();
            claims.Add(new Claim(ClaimTypes.UserData, profileId.ToString()));
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            await HttpContext.SignInAsync(claimsPrincipal);
        }

        private IActionResult AdminProfileView(AdminDto adminDto, bool succesfulyUpdated)
        {
            var trimmedAdress = adminDto.Adress.Split(' ');

            var model = new LicenseOwnerProfileModel
            {
                Name = adminDto.Name,
                LastName = adminDto.LastName,
                City = trimmedAdress[0],
                Street = trimmedAdress[1],
                HouseNumber = trimmedAdress[2],
                ZipCode = trimmedAdress[3],
                MobilePhone = adminDto.MobilePhone,
                NIP = adminDto.NIP,
                LicenseEndDate = adminDto.LicenseEndDate,
                LicenseStartDate = adminDto.LicenseStartDate,
                LicenseType = adminDto.LicenseType,
                ProfileImageUrl = adminDto.ProfileImageUrl,
                WasSuccesfulyUpdated = succesfulyUpdated
            };

            return View("~/Views/Profile/LicenseOwnerProfile.cshtml", model);
        }

        private IActionResult DentistProfileView(DentistDto dentistDto, bool succesfulyUpdated)
        {
            var trimmedAdress = dentistDto.Adress.Split(' ');

            var model = new DentistProfileModel
            {
                Name = dentistDto.Name,
                LastName = dentistDto.LastName,
                City = trimmedAdress[0],
                Street = trimmedAdress[1],
                HouseNumber = trimmedAdress[2],
                ZipCode = trimmedAdress[3],
                MobilePhone = dentistDto.MobilePhone,
                ProfileImageUrl = dentistDto.ProfileImageUrl,
                Description = dentistDto.Description,
                DoctorTitle = dentistDto.DoctorTitle,
                WasSuccesfulyUpdated = succesfulyUpdated
            };

            return View("~/Views/Profile/DentistProfile.cshtml", model);
        }

        private IActionResult PatientProfileView(PatientDto patientDto, bool succesfulyUpdated)
        {
            var model = new PatientProfileModel
            {
                Name = patientDto.Name,
                LastName = patientDto.LastName,
                MobilePhone = patientDto.MobilePhone,
                ProfileImageUrl = patientDto.ProfileImageUrl,
                WasSuccesfulyUpdated = succesfulyUpdated
            };

            return View("~/Views/Profile/PatientProfile.cshtml", model);
        }

        private IActionResult RecepcionistProfileView(ReceptionistDto receptionistDto, bool succesfulyUpdated)
        {
            var model = new RecepcionistProfileModel
            {
                Name = receptionistDto.Name,
                LastName = receptionistDto.LastName,
                ProfileImageUrl = receptionistDto.ProfileImageUrl,
                MobilePhone = receptionistDto.MobilePhone,
                WasSuccesfulyUpdated = succesfulyUpdated
            };

            return View("~/Views/Profile/RecepcionistProfile.cshtml", model);
        }

        #endregion
    }
}