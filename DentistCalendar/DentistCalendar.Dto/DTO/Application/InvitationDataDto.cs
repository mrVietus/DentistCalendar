using DentistCalendar.Common.Enums;
using System;

namespace DentistCalendar.Dto.DTO.Application
{
    public class InvitationDataDto
    {
        public int Id { get; set; }
        public string InvitationGuid { get; set; }
        public string InvitingProfileId { get; set; }
        public string InvitedProfileId { get; set; }
        public string InvitorName { get; set; }
        public int InvitingDentistOfficeId { get; set; }
        public AccountType InvitedAccountType { get; set; }
        public bool DiseableAccept { get; set; }
    }
}