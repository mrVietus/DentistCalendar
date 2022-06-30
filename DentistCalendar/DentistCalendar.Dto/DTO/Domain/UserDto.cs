using DentistCalendar.Common.Enums;
using System;

namespace DentistCalendar.Dto.DTO.Domain
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public Guid ProfileId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public AccountType AccountType { get; set; }
        public bool IsActive { get; set; }
    }
}