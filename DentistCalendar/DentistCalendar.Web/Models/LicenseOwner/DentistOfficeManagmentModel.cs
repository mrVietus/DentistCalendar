using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;

namespace DentistCalendar.Web.Models.LicenseOwner
{
    public class DentistOfficeManagmentModel
    {
        public IEnumerable<DentistOfficeDto> DentistOffices { get; set; }
        public DentistOfficeModel NewDentistOffice { get; set; }
    }
}