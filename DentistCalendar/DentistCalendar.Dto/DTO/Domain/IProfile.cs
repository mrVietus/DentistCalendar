using System;

namespace DentistCalendar.Dto.DTO.Domain
{
    public interface IProfile
    {
        Guid ProfileId { get; set; }
        string Email { get; set; }
        string Name { get; set; }
    }
}
