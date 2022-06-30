using DentistCalendar.Common.Enums;
using System;

namespace DentistCalendar.Dto.DTO.Domain
{
    public class InvitationDto
    {
        public int Id { get; set; }
        public string InvitationGuid { get; set; }
        public string InvitingName { get; set; }
        public string InvitingProfileId { get; set; }
        public string InvitingDentistOfficeId { get; set; }
        public AccountType InvitedAccountType { get; set; }
        public string InvitedProfileId { get; set; }
        public string InvitedEmail { get; set; }
    }
}