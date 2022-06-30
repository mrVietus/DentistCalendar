using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;

namespace DentistCalendar.Web.Models.Appointment
{
    public class MyAppointmentsModel
    {
        public IEnumerable<AppointmentDto> PatientAppoinments { get; set; }
    }
}
