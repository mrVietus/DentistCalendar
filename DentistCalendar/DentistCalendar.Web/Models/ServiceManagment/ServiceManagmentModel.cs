using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;

namespace DentistCalendar.Web.Models.ServiceManagment
{
    public class ServiceManagmentModel
    {
        public IEnumerable<DentistOfficeDto> DentistOffices { get; set; }
    }
}