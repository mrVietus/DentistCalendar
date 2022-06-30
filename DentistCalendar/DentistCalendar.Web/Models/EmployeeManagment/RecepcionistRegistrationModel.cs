using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Web.Models.EmployeeManagment
{
    public class RecepcionistRegistrationModel
    {
        [Required(ErrorMessage = "Proszę wypełnić pole email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Proszę wypełnić pole hasło tymczasowe.")]
        public string TemporaryPassword { get; set; }
        public int DentistOfficeId { get; set; }
    }
}