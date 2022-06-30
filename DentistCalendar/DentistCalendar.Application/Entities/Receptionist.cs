using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistCalendar.Core.Entities
{
    public class Receptionist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public Guid ProfileId { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string MobilePhone { get; set; }

        public string ProfileImageUrl { get; set; }

        public virtual DentistOffice DentistOffice { get; set; }

        public void Update(Receptionist receptionist)
        {
            Email = receptionist.Email;
            Name = receptionist.Name;
            LastName = receptionist.LastName;
            MobilePhone = receptionist.MobilePhone;
            ProfileImageUrl = receptionist.ProfileImageUrl;
        }

        public void UpdateDentistOffice(DentistOffice dentistOffice)
        {
            DentistOffice = dentistOffice;
        }
    }
}
