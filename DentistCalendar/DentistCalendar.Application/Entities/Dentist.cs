using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistCalendar.Core.Entities
{
    public class Dentist
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
        public string Adress { get; set; }
        [Required]
        public string MobilePhone { get; set; }

        public string ProfileImageUrl { get; set; }

        public string DoctorTitle { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<DentistDentistOffice> DentistDentistOffices { get; set; }
        public virtual ICollection<DentistService> DentistServices { get; set; }

        public void Update(Dentist dentist)
        {
            Name = dentist.Name;
            LastName = dentist.LastName;
            Adress = dentist.Adress;
            MobilePhone = dentist.MobilePhone;
            ProfileImageUrl = dentist.ProfileImageUrl;
            DoctorTitle = dentist.DoctorTitle;
            Description = dentist.Description;
        }

        public void UpdateAppointments(ICollection<Appointment> appointments)
        {
            Appointments = appointments;
        }

        public void UpdateDentistDentistOffices(ICollection<DentistDentistOffice> dentistDentistOffices)
        {
            DentistDentistOffices = dentistDentistOffices;
        }

        public void UpdateDentistServices(ICollection<DentistService> dentistServices)
        {
            DentistServices = dentistServices;
        }
    }
}