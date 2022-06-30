using DentistCalendar.Common.Enums;
using DentistCalendar.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DentistCalendar.Web.Controllers
{
    public class InvitationController : Controller
    {
        private readonly IInvitationService _invitationService;

        public InvitationController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        public async Task<IActionResult> AcceptInvitationFromLink(string invitationId, string email)
        {
            var accepted = await _invitationService.TryAcceptInvitationFromLinkAsync(invitationId, email);

            if (!accepted)
            {
                return PartialView("AcceptInvitationFailed");
            }

            return PartialView("AcceptInvitationSucceded");
        }

        [Authorize]
        public async Task<JsonResult> AcceptInvitation(string invitationGuid, string accountType)
        {
            if (accountType == null)
            {
                return Json(new { status = "error", modelErrors = $"[\"Nie ma takiego typu konta.\"]" });
            }

            if (string.IsNullOrEmpty(invitationGuid))
            {
                return Json(new { status = "error", modelErrors = $"[\"Puste id zaproszenia.\"]" });
            }

            string successLink;
            switch ((AccountType)Enum.Parse(typeof(AccountType), accountType))
            {
                case AccountType.Admin:
                    successLink = Url.Action("Index", "Admin");
                    break;
                case AccountType.Dentist:
                    successLink = Url.Action("Index", "Dentist");
                    break;
                case AccountType.Patient:
                    successLink = Url.Action("Index", "Patient");
                    break;
                default:
                    return Json(new { status = "error", modelErrors = $"[\"Nie ma takiego typu konta.\"]" });
            }

            var data = await _invitationService.TryAcceptInvitationAsync(invitationGuid, successLink);
            return Json(data);
        }

        [Authorize]
        public async Task<JsonResult> RejectInvitation(string invitationGuid, string accountType)
        {
            if (string.IsNullOrEmpty(invitationGuid))
            {
                return Json(new { status = "error", modelErrors = $"[\"Puste id zaproszenia.\"]" });
            }

            string successLink;
            switch ((AccountType)Enum.Parse(typeof(AccountType), accountType))
            {
                case AccountType.Admin:
                    successLink = Url.Action("Index", "Admin");
                    break;
                case AccountType.Dentist:
                    successLink = Url.Action("Index", "Dentist");
                    break;
                case AccountType.Patient:
                    successLink = Url.Action("Index", "Patient");
                    break;
                default:
                    return Json(new { status = "error", modelErrors = $"[\"Nie ma takiego typu konta.\"]" });
            }

            var data = await _invitationService.RejectInvitationAsync(invitationGuid, successLink);
            return Json(data);
        }
    }
}