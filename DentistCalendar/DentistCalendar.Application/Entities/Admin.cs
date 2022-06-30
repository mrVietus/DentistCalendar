using DentistCalendar.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DentistCalendar.Core.Entities
{
    public class Admin 
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
        [Required]
        public string NIP { get; set; }

        public string ProfileImageUrl { get; set; }

        public LicenseType LicenseType { get; set; }

        public DateTime LicenseStartDate { get; set; }
        public DateTime LicenseEndDate { get; set; }

        public virtual ICollection<DentistOffice> DentistOffices { get; set; }

        public void Update(Admin newAdminObject)
        {
            Adress = newAdminObject.Adress;
            LastName = newAdminObject.LastName;
            MobilePhone = newAdminObject.MobilePhone;
            Name = newAdminObject.Name;
            ProfileImageUrl = newAdminObject.ProfileImageUrl;
            NIP = newAdminObject.NIP;
        }

        public void UpdateLicense(DateTime newLicenseStartDate, DateTime newLicenseEndDate, LicenseType newLicenseType)
        {
            LicenseStartDate = newLicenseStartDate;
            LicenseEndDate = newLicenseEndDate;
            LicenseType = newLicenseType;
        }

        public void UpdateDentistOffices(ICollection<DentistOffice> dentistOffices)
        {
            DentistOffices = dentistOffices;
        }
    }
}
