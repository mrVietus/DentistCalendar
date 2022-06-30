using DentistCalendar.Common.Enums;

namespace DentistCalendar.Dto.DTO.Application
{
    public class RegistrationDto
    {
        public AccountType AccountType { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
        public string RegistrationConfirmationLink { get; set; }
    }
}