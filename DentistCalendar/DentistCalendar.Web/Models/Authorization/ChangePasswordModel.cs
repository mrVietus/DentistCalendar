using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Web.Models.Authorization
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Proszę wypełnić poprawnie pole stare hasło.")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Proszę wypełnić poprawnie pole nowe hasło.")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Pola nowe hasło oraz powtórz nowe hasło różnią się.")]
        public string RepeatNewPassword { get; set; }
    }
}