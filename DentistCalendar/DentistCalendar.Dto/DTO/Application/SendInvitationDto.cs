using System;
using DentistCalendar.Common.Enums;
using DentistCalendar.Dto.DTO.Domain;

namespace DentistCalendar.Dto.DTO.Application
{
    public class SendInvitationDto
    {
        public AccountType InvitingAccountType { get; set; }
        public AccountType InvitedAccountType { get; set; }
        public string InvitedEmail { get; set; }
        public string InvitedProfileId { get; set; }
        public string InvitingProfileId { get; set; }
        public DentistOfficeDto DentistOffice { get; set; }
        public string AcceptInvitationLink { get; set; }
        public Guid InvitationGuid { get; set; }
    }
}
