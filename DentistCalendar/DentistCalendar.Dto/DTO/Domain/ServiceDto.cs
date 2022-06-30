using System;
using System.Collections.Generic;

namespace DentistCalendar.Dto.DTO.Domain
{
    public class ServiceDto
    {
        public string Name { get; set; }
        public DentistOfficeDto DentistOffice { get; set; }
        public IEnumerable<DentistDto> Dentists { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public TimeSpan Time { get; set; }
        public int Id { get; set; }
    }
}
