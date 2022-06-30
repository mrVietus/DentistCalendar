using DentistCalendar.Common.Logger;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services.Impl
{
    public class LicenseOwnerService : ILicenseOwnerService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IDentistOfficeRepository _dentistOfficeRepository;
        private readonly ILoggerService _loggerService;

        public LicenseOwnerService(IAdminRepository adminRepository, IDentistOfficeRepository dentistOfficeRepository, ILoggerService loggerService)
        {
            _adminRepository = adminRepository;
            _dentistOfficeRepository = dentistOfficeRepository;
            _loggerService = loggerService;
        }

        public async Task<IEnumerable<DentistOfficeDto>> GetDentistOfficesAsync(string profileId)
        {
            _loggerService.Info("Getting dentist offices.");

            try
            {
                var adminDtoWithDentistOffices = await _adminRepository.GetByProfileIdWithDentistOfficesAsync(Guid.Parse(profileId));

                return adminDtoWithDentistOffices.DentistOffices;
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist offices failed.", ex);
                return null;
            }
        }

        public async Task<bool> AddDentistOfficeAsync(DentistOfficeDto dentistOffice, string profileId)
        {
            _loggerService.Info("Adding dentist office.");

            try
            {
                dentistOffice.Admin = await _adminRepository.GetByProfileIdAsync(Guid.Parse(profileId));
                return await _adminRepository.CreateDentistOfficeAsync(dentistOffice);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Adding dentist office failed.", ex);
                return false;
            }
        }

        public async Task<DentistOfficeDto> GetDentistOfficeByIdAsync(int dentistOfficeId)
        {
            _loggerService.Info($"Getting dentist office with id {dentistOfficeId}.");

            try
            {
                return await _dentistOfficeRepository.GetByIdAsync(dentistOfficeId);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist office with id {dentistOfficeId} failed.", ex);
                return null; ;
            }
        }

        public async Task<bool> EditDentistOfficeAsync(DentistOfficeDto dentistOffice)
        {
            _loggerService.Info($"Updating dentist office with id {dentistOffice.Id}.");

            try
            {
                return await _dentistOfficeRepository.UpdateAsync(dentistOffice);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Update dentist office with id {dentistOffice.Id} failed.", ex);
                return false; ;
            }
        }

        public async Task<bool> RemoveDentistOfficeAsync(int dentistOfficeId)
        {
            _loggerService.Info($"Removeing dentist office with id {dentistOfficeId}.");

            try
            {
                return await _dentistOfficeRepository.RemoveAsync(dentistOfficeId);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"remove dentist office with id {dentistOfficeId} failed.", ex);
                return false; ;
            }
        }
    }
}
