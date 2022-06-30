﻿using System;

namespace DentistCalendar.Dto.DTO.Domain
{
    public class DentistDto : IProfile
    {
        public Guid ProfileId { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public string Adress { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DoctorTitle { get; set; }
        public string LastName { get; set; }
        public string MobilePhone { get; set; }
        public string ProfileImageUrl { get; set; }
    }
}