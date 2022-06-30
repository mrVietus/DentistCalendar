using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Web.Models.Profile
{
    public class PatientProfileModel
    {
        [Required(ErrorMessage = "Imie jest wymagane.")]
        [StringLength(25, ErrorMessage = "Twoje imię nie powinno przekraczać 25 znaków.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [StringLength(25, ErrorMessage = "Twoje nazwisko nie powinno przekraczać 25 znaków.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany.")]
        [RegularExpression(@"^(\+48\s+)?\d{3}(\s*|\-)\d{3}(\s*|\-)\d{3}$", ErrorMessage = "Błędny format numeru telefonu.")]
        public string MobilePhone { get; set; }

        public string AvatarImageName { get; set; }
        public bool WasSuccesfulyUpdated { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}