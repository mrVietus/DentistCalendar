using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Web.Models.LicenseOwner
{
    public class DentistOfficeModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nazwa placówki jest wymagana.")]
        [StringLength(35, ErrorMessage = "Nazwa placówki nie powinna przekraczać 35 znaków.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email do placówki jest wymagany.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany.")]
        [RegularExpression(@"^(\+48\s+)?\d{3}(\s*|\-)\d{3}(\s*|\-)\d{3}$", ErrorMessage = "Błędny format numeru telefonu.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Miasto jest wymagane.")]
        [StringLength(25, ErrorMessage = "Nazwa twojego miasta nie powinna przekraczać 25 znaków.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Ulica jest wymagana.")]
        [StringLength(25, ErrorMessage = "Nazwa twojej ulicy nie powinna przekraczać 25 znaków.")]
        public string Street { get; set; }

        [Required(ErrorMessage = "Numer domu jest wymagany.")]
        public string HouseNumber { get; set; }

        public string ApartamentNumber { get; set; }

        [Required(ErrorMessage = "Kod pocztowy jest wymagany.")]
        [RegularExpression(@"^\d{2}-\d{3}$", ErrorMessage = "Nieprawidłowy format kodu pocztowego.")]
        public string ZipCode { get; set; }

        [StringLength(500, ErrorMessage = "Prosimy zmieścić się w 500 znakach.")]
        public string AboutUs { get; set; }
    }
}