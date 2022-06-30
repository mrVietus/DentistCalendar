using DentistCalendar.Common.Enums;
using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Infrastructure.Services;
using DentistCalendar.Web.Models.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DentistCalendar.Web.Controllers
{
    [Route("auth")]
    public class AuthorizationController : Controller
    {
        private readonly IUserService _userService;

        public AuthorizationController(IUserService userService)
        {
            _userService = userService;
        }

        #region Login

        [Route("login")]
        [AllowAnonymous]
        public IActionResult LogIn()
        {
            return View(new LogInModel());
        }

        [Route("login")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogIn(LogInModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var authDto = await _userService.AuthenticateAsync(model.Email, model.Password);
            if (authDto == null)
            {
                ModelState.AddModelError("InvalidCredentials", "Błędny login lub hasło.");
                return View(model);
            }

            if (!authDto.IsActive)
            {
                ModelState.AddModelError("UserIsNotConfirmed", "Aktywuj konto. Link aktywacyjny powinien być na twoim mailu.");
                return View(model);
            }

            return await SisgnInAndRedirectToUsersController(authDto);
        }

        #endregion

        #region Registration

        [Route("register")]
        [AllowAnonymous]
        public IActionResult Register()
        {
            var model = new RegisterModel()
            {
                AgreeTermsAndConditions = false
            };

            return View(model);
        }

        [Route("register")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registration = new RegistrationDto
            {
                AccountType = (AccountType)model.AccountType,
                Email = model.Email,
                Password = model.Password,
                RepeatPassword = model.RepeatPassword,
                RegistrationConfirmationLink = Url.Action("ConfirmEmailAsync", "Authorization", new { registrationGuid = "HereShouldBeGuid", email = "HereShouldBeEmail" }, protocol: HttpContext.Request.Scheme)
            };

            if (!await _userService.TryRegisterAsync(registration))
            {
                ModelState.AddModelError("UserAlreadyExistsInDb", $"Urzytkownik o adresie o adresie e-mail: {model.Email} już istnieje.");
                return View(model);
            }

            return View("~/Views/Authorization/SuccesfulyRegistered.cshtml", model.Email);
        }

        [Route("confirmemail")]
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmailAsync(string registrationGuid, string email)
        {
            if (!await _userService.ConfirmEmailAsync(registrationGuid, email))
            {
                return View("~/Views/Authorization/ConfirmEmailAsyncFailed.cshtml");
            }

            return View();
        }

        #endregion

        #region ChangePassword

        [Route("changepassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Route("changepassword")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var changePasswordDto = new ChangePasswordDto
            {
                Email = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.Email).Select(c => c.Value).SingleOrDefault(),
                NewPassword = model.NewPassword,
                OldPassword = model.OldPassword
            };

            if (!await _userService.ChangePasswordAsync(changePasswordDto))
            {
                ModelState.AddModelError("ChangePasswordFailed", "Obecne hasło jest złe.");
                return View(model);
            }

            return View("~/Views/Authorization/SuccesfulyChangedPassword.cshtml");
        }

        #endregion

        [Route("accessdenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("logout")]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        #region PrivateMethods
        private async Task<IActionResult> SisgnInAndRedirectToUsersController(AuthenticationDto authDto)
        {
            await HttpContext.SignInAsync(authDto.ClaimsPrincipal);

            switch (authDto.AccountType)
            {
                case AccountType.Admin:
                    return RedirectToAction("Index", "LicenseOwner");
                case AccountType.Dentist:
                    return RedirectToAction("Index", "Home");
                case AccountType.Patient:
                    return RedirectToAction("Index", "Patient");
                case AccountType.Receptionist:
                    return RedirectToAction("Index", "Receptionist");
                case AccountType.SysAdmin:
                    return RedirectToAction("Index", "Admin");
                default:
                    return RedirectToAction("Index", "Home");
            }
        }
        #endregion
    }
}