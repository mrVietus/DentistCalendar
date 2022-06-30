using DentistCalendar.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace DentistCalendar.Web.Models.Profile
{
    public class LicenseOwnerProfileModel
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

        [Required(ErrorMessage = "Numer itentyfikacji podatkowej (NIP) jest wymagany.")]
        [RegularExpression(@"^((\d{3}[- ]\d{3}[- ]\d{2}[- ]\d{2})|(\d{3}[- ]\d{2}[- ]\d{2}[- ]\d{3}))$", ErrorMessage = "Nieprawidłowy format numeru NIP.")]
        public string NIP { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany.")]
        [RegularExpression(@"^(\+48\s+)?\d{3}(\s*|\-)\d{3}(\s*|\-)\d{3}$", ErrorMessage = "Błędny format numeru telefonu.")]
        public string MobilePhone { get; set; }

        public string AvatarImageName { get; set; }
        public LicenseType LicenseType { get; set; }
        public DateTime LicenseStartDate { get; set; }
        public DateTime LicenseEndDate { get; set; }
        public bool WasSuccesfulyUpdated { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}