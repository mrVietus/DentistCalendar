using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistCalendar.Core.Entities
{
    public class Service
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public double Price { get; set; }

        public TimeSpan Time { get; set; }

        public string Description { get; set; }

        public virtual ICollection<DentistService> DentistServices { get; set; }
        public virtual DentistOffice DentistOffice { get; set; }

        public void Update(Service service)
        {
            Description = service.Description;
            Name = service.Name;
            Price = service.Price;
            Time = service.Time;
        }

        public void UpdateDentistServices(ICollection<DentistService> dentistServices)
        {
            DentistServices = dentistServices;
        }

        public void UpdateDentistOffice(DentistOffice dentistOffice)
        {
            DentistOffice = dentistOffice;
        }
    }
}