using DentistCalendar.Common.Enums;
using System;
using System.Collections.Generic;

namespace DentistCalendar.Dto.DTO.Domain
{
    public class AdminDto : IProfile
    {
        public Guid ProfileId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string MobilePhone { get; set; }
        public string ProfileImageUrl { get; set; }
        public string NIP { get; set; }
        public LicenseType LicenseType { get; set; }
        public DateTime LicenseStartDate { get; set; }
        public DateTime LicenseEndDate { get; set; }

        public IEnumerable<DentistOfficeDto> DentistOffices { get; set; }
    }
}
