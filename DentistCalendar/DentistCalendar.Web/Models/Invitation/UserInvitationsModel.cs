using DentistCalendar.Dto.DTO.Application;
using System.Collections.Generic;

namespace DentistCalendar.Web.Models.Invitation
{
    public class UserInvitationsModel
    {
        public IEnumerable<InvitationDataDto> Invitations { get; set; }
    }
}