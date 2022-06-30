using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistCalendar.Core.Entities
{
    public class DentistOffice
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Adress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        public string AboutUs { get; set; }

        public virtual Admin Admin { get; set; }
        public virtual ICollection<Receptionist> Receptionists { get; set; }
        public virtual ICollection<DentistDentistOffice> DentistDentistOffices { get; set; }
        public virtual ICollection<Service> Services { get; set; }

        public void Update(DentistOffice dentistOffice)
        {
            Name = dentistOffice.Name;
            Adress = dentistOffice.Adress;
            City = dentistOffice.City;
            Email = dentistOffice.Email;
            Phone = dentistOffice.Phone;
            AboutUs = dentistOffice.AboutUs;
        }

        public void UpdateReceptionists(ICollection<Receptionist> receptionists)
        {
            Receptionists = receptionists;
        }

        public void UpdateDentistDentistOffices(ICollection<DentistDentistOffice> dentistDentistOffices)
        {
            DentistDentistOffices = dentistDentistOffices;
        }

        public void UpdateServices(ICollection<Service> services)
        {
            Services = services;
        }
    }
}