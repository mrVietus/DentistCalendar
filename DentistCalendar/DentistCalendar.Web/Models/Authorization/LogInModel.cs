using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Web.Models.Authorization
{
    public class LogInModel
    {
        [Required(ErrorMessage = "Proszę wypełnić poprawnie pole email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Proszę wypełnić poprawnie pole password.")]
        public string Password { get; set; }
    }
}