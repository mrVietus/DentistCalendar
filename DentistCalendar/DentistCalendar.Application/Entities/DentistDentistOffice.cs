namespace DentistCalendar.Core.Entities
{
    public class DentistDentistOffice
    {
        public int DentistId { get; set; }
        public Dentist Dentist { get; set; }
        public int DentistOfficeId { get; set; }
        public DentistOffice DentistOffice { get; set; }
    }
}