using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistCalendar.Core.Entities
{
    public class Patient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid ProfileId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string MobilePhone { get; set; }

        public string ProfileImageUrl { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }

        public void Update(Patient patient)
        {
            Name = patient.Name;
            LastName = patient.LastName;
            MobilePhone = patient.MobilePhone;
            ProfileImageUrl = patient.ProfileImageUrl;
        }

        public void UpdateAppointments(ICollection<Appointment> appointments)
        {
            Appointments = appointments;
        }
    }
}
