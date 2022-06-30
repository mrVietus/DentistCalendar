using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Web.Models.Profile
{
    public class DentistProfileModel
    {
        [Required(ErrorMessage = "Imie jest wymagane.")]
        [StringLength(25, ErrorMessage = "Twoje imię nie powinno przekraczać 25 znaków.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [StringLength(25, ErrorMessage = "Twoje nazwisko nie powinno przekraczać 25 znaków.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Miasto jest wymagane.")]
        [StringLength(25, ErrorMessage = "Nazwa twojego miasta nie powinna przekraczać 25 znaków.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Ulica jest wymagana.")]
        [StringLength(25, ErrorMessage = "Nazwa twojej ulicy nie powinna przekraczać 25 znaków.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Numer domu jest wymagany.")]
        public string HouseNumber { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany.")]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Nieprawidłowy format kodu pocztowego.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany.")]
        [RegularExpression(@"^(\+48\s+)?\d{3}(\s*|\-)\d{3}(\s*|\-)\d{3}$", ErrorMessage = "Błędny format numeru telefonu.")]
        public string MobilePhone { get; set; }
        
        [StringLength(10, ErrorMessage = "Twoje imię nie powinno przekraczać 10 znaków.")]
        public string DoctorTitle { get; set; }

        public string Description { get; set; }

        public string AvatarImageName { get; set; }

        public string ProfileImageUrl { get; set; }
        public bool WasSuccesfulyUpdated { get; internal set; }
    }
}