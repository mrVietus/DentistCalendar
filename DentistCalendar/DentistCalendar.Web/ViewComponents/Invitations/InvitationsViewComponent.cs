using DentistCalendar.Infrastructure.Services;
using DentistCalendar.Web.Models.Invitation;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DentistCalendar.Web.ViewComponents.Invitations
{
    public class InvitationsViewComponent : ViewComponent
    {
        private readonly string componentView = "~/Views/Shared/Components/Invitations/Invitations.cshtml";

        private readonly IInvitationService _invitationService;

        public InvitationsViewComponent(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userProfileId = HttpContext.User.Claims.Where(c => c.Type == ClaimTypes.UserData).Select(c => c.Value).SingleOrDefault();

            if (string.IsNullOrEmpty(userProfileId))
            {
                return View(componentView);
            }

            var model = new UserInvitationsModel()
            {
                Invitations = await _invitationService.GetInvitationsForProfileAsync(userProfileId)
            };

            return View(componentView, model);
        }
    }
}