using DentistCalendar.Dto.DTO.Domain;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace DentistCalendar.Web.Models.ServiceManagment
{
    public class AddEditServiceModel
    {
        public List<SelectListItem> AvaliableDentists { get; set; }
        public ServiceDto Service { get; set; }
        public IEnumerable<int> AssignedDentistListIds { get; set; }
        public int DentistOfficeId { get; set; }
        public List<SelectListItem> AvaliableServiceTime { set; get; }
    }
}