using System.Collections.Generic;

namespace DentistCalendar.Dto.DTO.Application
{
    public class SendEmailDto
    {
        public string From { get; set; }
        public ICollection<string> To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
    }
}