using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services.Impl
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IDentistOfficeRepository _dentistOfficeRepository;
        private readonly IDentistRepository _dentistRepository;
        private readonly IProfileService _profileService;
        private readonly IServiceRepository _serviceRepository;

        public AppointmentService(IDentistOfficeRepository dentistOfficeRepository, IDentistRepository dentistRepository,
            IAppointmentRepository appointmentRepository, IServiceRepository serviceRepository, IProfileService profileService)
        {
            _appointmentRepository = appointmentRepository;
            _dentistOfficeRepository = dentistOfficeRepository;
            _dentistRepository = dentistRepository;
            _serviceRepository = serviceRepository;
            _profileService = profileService;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsForPatient(string userProfileId)
        {
            var patient = (PatientDto)await _profileService.GetProfileForUser(userProfileId, "Patient");

            return await _appointmentRepository.GetAllForPatientAsync(patient);
        }

        public async Task<IEnumerable<string>> GetAvaliableCities()
        {
            return await _dentistOfficeRepository.GetDentistOficesCitiesAsync();
        }

        public async Task<IEnumerable<DentistDto>> GetAvaliableDentistsAsync(int serviceId, int dentistOfficeId)
        {
            var dentists = await _dentistRepository.GetWithServiceAndDentistOfficeAsync(serviceId, dentistOfficeId);

            return dentists;
        }

        public async Task<IEnumerable<DentistOfficeDto>> GetAvaliableOfficesAsync(string cityName)
        {
            return await _dentistOfficeRepository.GetDentistOficesForCityAsync(cityName);
        }

        public async Task<IEnumerable<string>> GetAvaliableServiceHoursAsync(int serviceId, int dentistId, int dentistOfficeId, string date)
        {
            var serviceDate = DateTime.ParseExact(date.Replace('.', '/'), "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var scheduledServices = await _appointmentRepository.GetForDentistWithDateAsync(dentistId, serviceDate);
            var service = await _serviceRepository.GetByIdAsync(serviceId);

            if (!scheduledServices.Any())
            {
                return CreateAllAvaliableServiceHours(service.Time);
            }

            return Enumerable.Empty<string>();
        }

        public async Task<IEnumerable<ServiceDto>> GetAvaliableServicesAsync(int dentistOfficeId)
        {
            var dentistOffice = await _dentistOfficeRepository.GetWithServicesByIdAsync(dentistOfficeId);

            if (dentistOffice == null || dentistOffice.Services == null || !dentistOffice.Services.Any())
            {
                return Enumerable.Empty<ServiceDto>();
            }

            return dentistOffice.Services;
        }

        public async Task<object> ScheduleServiceAsync(int serviceId, int dentistId, int dentistOfficeId,
            string serviceTime, string userProfileId, string successLink)
        {
            if (string.IsNullOrEmpty(serviceTime) || string.IsNullOrEmpty(successLink) || string.IsNullOrEmpty(userProfileId))
            {
                return new { status = "error", modelErrors = $"[\"Wystąpił nieoczekiwany błąd podczas zamawiania wizyty.\"]" };
            }

            var patient = (PatientDto)await _profileService.GetProfileForUser(userProfileId, "Patient");

            if (patient == null)
            {
                return new { status = "error", modelErrors = $"[\"Nie istnieje profil pacjenta.\"]" };
            }

            var serviceDate = DateTime.ParseExact(serviceTime.Replace('.', '/'), "dd/MM/yyyy:HH:mm", CultureInfo.InvariantCulture);

            if (!await _appointmentRepository.CreateAsync(patient.Id, dentistId, serviceId, serviceDate))
            {
                return new { status = "error", modelErrors = $"[\"Wystąpił nieoczekiwany błąd podczas zamawiania wizyty.\"]" };
            }

            return new { status = "success", url = successLink };
        }

        private IEnumerable<string> CreateAllAvaliableServiceHours(TimeSpan time)
        {
            var timeList = new List<string>();

            for (int i = 9; i <= 20; i++)
            {
                timeList.Add($"{i}:00");
            }

            return timeList;
        }
    }
}
