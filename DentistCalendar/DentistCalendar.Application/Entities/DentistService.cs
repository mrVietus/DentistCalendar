namespace DentistCalendar.Core.Entities
{
    public class DentistService
    {
        public int? DentistId { get; set; }
        public Dentist Dentist { get; set; }
        public int? ServiceId { get; set; }
        public Service Service { get; set; }
    }
}