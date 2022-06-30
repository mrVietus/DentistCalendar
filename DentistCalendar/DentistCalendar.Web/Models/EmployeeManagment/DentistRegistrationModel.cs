using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Web.Models.EmployeeManagment
{
    public class DentistRegistrationModel
    {
        [Required(ErrorMessage = "Proszę wypełnić pole email.")]
        public string Email { get; set; }
        public int DentistOfficeId { get; set; }
    }
}