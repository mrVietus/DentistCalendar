using System;

namespace DentistCalendar.Dto.DTO.Domain
{
    public class ReceptionistDto : IProfile
    {
        public int Id { get; set; }
        public Guid ProfileId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }    
        public string ProfileImageUrl { get; set; }    
        public DentistOfficeDto DentistOffice { get; set; }
    }
}
