using DentistCalendar.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistCalendar.Core.Entities
{
    public class Invitation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Guid { get; set; }
        [Required]
        public string InvitingName { get; set; }
        [Required]
        public string InvitingProfileId { get; set; }
        public string InvitingDentistOfficeId { get; set; }
        [Required]
        public AccountType InvitedAccountType{ get; set; }
        public string InvitedProfileId { get; set; }
        [Required]
        public string InvitedEmail { get; set; }
    }
}