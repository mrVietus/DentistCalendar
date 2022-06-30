using DentistCalendar.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Core.Entities
{
    public class User
    {
        [Key]
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Salt { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public AccountType AccountType { get; set; }

        public Guid ProfileId { get; set; }

        public void Update(User user)
        {
            Email = user.Email;
            AccountType = user.AccountType;
            ProfileId = ProfileId;
        }

        public void UpdatePassword(string newPassword, string newSalt)
        {
            Password = newPassword;
            Salt = newSalt;
        }
    }
}