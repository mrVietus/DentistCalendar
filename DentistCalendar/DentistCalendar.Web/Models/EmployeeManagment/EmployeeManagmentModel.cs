using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;

namespace DentistCalendar.Web.Models.EmployeeManagment
{
    public class EmployeeManagmentModel
    {
        public IEnumerable<DentistOfficeDto> DentistOffices { get; set; }
    }
}
