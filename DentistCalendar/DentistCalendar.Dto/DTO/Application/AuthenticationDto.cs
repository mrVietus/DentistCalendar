using DentistCalendar.Common.Enums;
using System.Security.Claims;

namespace DentistCalendar.Dto.DTO.Application
{
    public class AuthenticationDto
    {
        public ClaimsPrincipal ClaimsPrincipal { get; set; }
        public bool IsActive { get; set; }
        public AccountType AccountType { get; set; }
    }
}