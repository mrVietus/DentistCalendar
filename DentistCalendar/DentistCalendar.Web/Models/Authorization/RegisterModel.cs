using DentistCalendar.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Web.Models.Authorization
{
    public class RegisterModel
    {
        [RegularExpression("^[1-3]$", ErrorMessage = "Proszę wybrać typ konta.")]
        public int AccountType { get; set; }

        [Required(ErrorMessage = "Proszę wypełnić poprawnie pole email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Proszę wypełnić poprawnie pole password.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Pola hasło oraz powtórz hasło różnią się.")]
        public string RepeatPassword { get; set; }

        [RegularExpression("True", ErrorMessage = "Aby założyć konto trzeba się zgodzić na politykę prywatności.")]
        public bool AgreeTermsAndConditions { get; set; }
    }
}