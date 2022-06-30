using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistCalendar.Core.Entities
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public virtual Dentist Dentist { get; set; }
        public virtual DentistOffice DentistOffice { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Service Service { get; set; }

        public void Update(Appointment appointment)
        {
            StartDate = appointment.StartDate;
            Dentist = appointment.Dentist;
            DentistOffice = appointment.DentistOffice;
            Patient = appointment.Patient;
            Service = appointment.Service;
        }
    }
}