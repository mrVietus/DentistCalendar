using System.Collections.Generic;

namespace DentistCalendar.Dto.DTO.Domain
{
    public class DentistOfficeDto
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string AboutUs { get; set; }
        public AdminDto Admin { get; set; }
        public string Adress { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public IEnumerable<ReceptionistDto> Receptionists { get; set; }
        public IEnumerable<DentistDto> Dentists { get; set; }
        public IEnumerable<ServiceDto> Services { get; set; }
    }
}