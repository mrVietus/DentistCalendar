using DentistCalendar.Common.Logger;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Repositories;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services.Impl
{
    public class ServiceManagmentService : IServiceManagmentService
    {
        private readonly IDentistRepository _dentistRepository;
        private readonly IDentistOfficeRepository _dentistOfficeRepository;
        private readonly ILicenseOwnerService _licenseOwnerService;
        private readonly ILoggerService _loggerService;
        private readonly IServiceRepository _serviceRepository;

        public ServiceManagmentService(IDentistRepository dentistRepository,
                                       IDentistOfficeRepository dentistOfficeRepository,
                                       ILicenseOwnerService licenseOwnerService,
                                       ILoggerService loggerService,
                                       IServiceRepository serviceRepository)
        {
            _dentistRepository = dentistRepository;
            _dentistOfficeRepository = dentistOfficeRepository;
            _licenseOwnerService = licenseOwnerService;
            _loggerService = loggerService;
            _serviceRepository = serviceRepository;
        }

        public async Task<IEnumerable<DentistDto>> GetDentistsForServiceAsync(int dentistOfficeId)
        {
            var dentistOfficeWithDentists = await _dentistOfficeRepository.GetByIWithDentistsAsync(dentistOfficeId);

            if (dentistOfficeWithDentists == null)
            {
                return null;
            }

            return dentistOfficeWithDentists.Dentists;
        }

        public async Task<ServiceDto> GetServiceByIdAsync(int Id)
        {
            return await _serviceRepository.GetByIdAsync(Id);
        }

        public async Task<IEnumerable<DentistOfficeDto>> GetServicesAsync(string profileId)
        {
            var dentistOffices = await _licenseOwnerService.GetDentistOfficesAsync(profileId);

            if (dentistOffices != null)
            {
                var dentistOfficesWithFilledData = new List<DentistOfficeDto>();

                foreach (var dentistOffice in dentistOffices)
                {
                    dentistOfficesWithFilledData.Add(await _dentistOfficeRepository.GetWithServicesByIdAsync(dentistOffice.Id));
                }

                return dentistOfficesWithFilledData;
            }

            return null;
        }

        public async Task<object> TryAddServiceAsync(int dentistOfficeId, ServiceDto service, IEnumerable<int> assignedDentists, string successLink)
        {
            if (service.Time.TotalSeconds == 0)
            {
                return new { status = "error", modelErrors = $"[\"Wybierz czas trwania usługi.\"]" };
            }

            var dentistOffice = await _licenseOwnerService.GetDentistOfficeByIdAsync(dentistOfficeId);

            if (dentistOffice == null)
            {
                return new { status = "error", modelErrors = $"[\"Wtstąpił nieoczekiwany błąd.\"]" };
            }

            var response = await _serviceRepository.CreateAsync(service);
            var connectionBetweenServiceAndDentistsWasCreated = await _serviceRepository.AddAssignedDentistsAsync(assignedDentists, response.Serviceid);

            if (!response.WasServiceAdded && connectionBetweenServiceAndDentistsWasCreated)
            {
                return new { status = "error", modelErrors = $"[\"Wtstąpił nieoczekiwany błąd podczas dodawania serwisu.\"]" };
            }

            if (!await _dentistOfficeRepository.AddServiceAsync(response.Serviceid, dentistOfficeId))
            {
                await _serviceRepository.RemoveAsync(response.Serviceid);
                return new { status = "error", modelErrors = $"[\"Wtstąpił nieoczekiwany błąd podczas dodawania serwisu.\"]" };
            }

            return new { status = "success", url = successLink };
        }

        public async Task<object> TryEditServiceAsync(ServiceDto service, IEnumerable<int> assignedDentists, string successLink)
        {
            if (service.Time.TotalSeconds == 0)
            {
                return new { status = "error", modelErrors = $"[\"Wybierz czas trwania usługi.\"]" };
            }

            if (!await _serviceRepository.EditAssignedDentistsAsync(assignedDentists, service.Id))
            {
                return new { status = "error", modelErrors = $"[\"Wtstąpił nieoczekiwany błąd podczas edycji serwisu.\"]" };
            }

            if (!await _serviceRepository.UpdateAsync(service))
            {
                return new { status = "error", modelErrors = $"[\"Wtstąpił nieoczekiwany błąd podczas edycji serwisu.\"]" };
            }

            return new { status = "success", url = successLink };
        }

        public async Task<bool> TryRemoveServiceAsync(int serviceId, int dentistOfficeId)
        {
            if (!await _dentistOfficeRepository.RemoveServiceAsync(serviceId, dentistOfficeId))
            {
                return false;
            }

            return await _serviceRepository.RemoveAsync(serviceId);
        }
    }
}