using System;

namespace DentistCalendar.Dto.DTO.Domain
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public virtual DentistDto Dentist { get; set; }
        public virtual DentistOfficeDto DentistOffice { get; set; }
        public virtual ServiceDto Service { get; set; }
    }
}
