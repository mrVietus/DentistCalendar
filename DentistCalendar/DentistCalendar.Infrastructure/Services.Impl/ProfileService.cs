using DentistCalendar.Common.Enums;
using DentistCalendar.Common.Logger;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services.Impl
{
    public class ProfileService : IProfileService
    {
        private readonly ILoggerService _loggerService;
        private readonly IAdminRepository _adminRepository;
        private readonly IDentistRepository _dentistRepository;
        private readonly IDentistOfficeRepository _dentistOfficeRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IReceptionistRepository _receptionistRepository;
        private readonly IUserRepository _userRepository;

        public ProfileService(ILoggerService loggerService, IAdminRepository adminRepository,
                              IDentistRepository dentistRepository, IDentistOfficeRepository dentistOfficeRepository,
                              IPatientRepository patientRepository, IReceptionistRepository receptionistRepository,
                              IUserRepository userRepository)
        {
            _loggerService = loggerService;
            _adminRepository = adminRepository;
            _dentistRepository = dentistRepository;
            _dentistOfficeRepository = dentistOfficeRepository;
            _patientRepository = patientRepository;
            _receptionistRepository = receptionistRepository;
            _userRepository = userRepository;
        }

        public Task<IProfile> GetProfileForUser(string userProfileId, string accountType)
        {
            if (string.IsNullOrEmpty(userProfileId))
            {
                _loggerService.Error($"Getting profile failed because profile id was null or empty.");
                return Task.FromResult<IProfile>(null);
            }

            _loggerService.Info($"Getting profile for guid {userProfileId} and account type: {accountType} ");

            try
            {
                var accountTypeEnum = (AccountType)Enum.Parse(typeof(AccountType), accountType);
                var profileId = Guid.Parse(userProfileId);
                return GetProfile(accountTypeEnum, profileId);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting profile for guid {userProfileId} and account type: {accountType}  failed: ", ex);
                return Task.FromResult<IProfile>(null);
            }
        }

        public Task<IProfile> GetProfileForUser(string userProfileId, AccountType accountType)
        {
            if (string.IsNullOrEmpty(userProfileId))
            {
                _loggerService.Error($"Getting profile failed because profile id was null or empty.");
                return Task.FromResult<IProfile>(null);
            }

            _loggerService.Info($"Getting profile for guid {userProfileId} and account type: {accountType} ");

            try
            {
                var profileId = Guid.Parse(userProfileId);
                return GetProfile(accountType, profileId);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting profile for guid {userProfileId} and account type: {accountType}  failed: ", ex);
                return Task.FromResult<IProfile>(null);
            }
        }

        public async Task<string> TryCreateProfileAsync(IProfile profile, IFormFile avatar)
        {
            _loggerService.Info($"Trying to create profile.");

            _loggerService.Info($"Trying to save avatar in blob storage.");
            var avatarUrl = "noAvatar";
            _loggerService.Info($"Save avatar in blob storage succeded.");

            if (string.IsNullOrEmpty(avatarUrl))
            {
                _loggerService.Error($"Avatar url was null. Creation of profile failed.");
            }

            try
            {
                profile.ProfileId = Guid.NewGuid();
                var wasCreatedSuccesfuly = false;
                wasCreatedSuccesfuly = await CreateProfileAsync(profile, avatarUrl);

                if (!wasCreatedSuccesfuly)
                {
                    _loggerService.Error($"Profile wasn't created succesfully. Creation of profile failed. Check previous logs.");
                    return null;
                }

                if (!await _userRepository.AddProfileId(profile.Email, profile.ProfileId))
                {
                    _loggerService.Error($"Cannot assign profileId: {profile.ProfileId} to profile with email: {profile.Email}. Removeing profile from db.");
                    await RemoveProfileAsync(profile);
                    return null;
                }

                _loggerService.Info($"Profile was created succesfuly.");
                return await Task.FromResult(profile.ProfileId.ToString());
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Creating profile failed: ", ex);
                return null;
            }
        }

        public async Task<bool> TryUpdateProfileAsync(IProfile profile, IFormFile avatar)
        {
            _loggerService.Info($"Trying to update avatar in blob storage.");
            var avatarUrl = "noAvatar";
            _loggerService.Info($"Update avatar in blob storage succeded.");

            if (string.IsNullOrEmpty(avatarUrl))
            {
                _loggerService.Error($"Avatar url was null. Update of profile failed.");
            }

            if (profile == null)
            {
                _loggerService.Error($"Profile was null. Update of profile failed.");
                return false;
            }

            try
            {
                return await UpdateProfileAsync(profile, avatarUrl);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Updating profile failed: ", ex);
                return false;
            }
        }

        #region PrivateMethods

        #region GetProfile and private methods
        private Task<IProfile> GetProfile(AccountType accountTypeEnum, Guid profileId)
        {
            switch (accountTypeEnum)
            {
                case AccountType.Admin:
                    return GetAdminProfileAsync(profileId);
                case AccountType.Dentist:
                    return GetDentistProfile(profileId);
                case AccountType.Patient:
                    return GetPatientProfile(profileId);
                case AccountType.Receptionist:
                    return GetReceptionistProfile(profileId);
                default:
                    return null;
            }
        }

        private async Task<IProfile> GetAdminProfileAsync(Guid userProfileId)
        {
            return await _adminRepository.GetByProfileIdAsync(userProfileId);
        }

        private async Task<IProfile> GetDentistProfile(Guid userProfileId)
        {
            return await _dentistRepository.GetByProfileIdAsync(userProfileId);
        }

        private async Task<IProfile> GetPatientProfile(Guid userProfileId)
        {
            return await _patientRepository.GetByProfileIdAsync(userProfileId);
        }

        private async Task<IProfile> GetReceptionistProfile(Guid userProfileId)
        {
            return await _receptionistRepository.GetByProfileIdAsync(userProfileId);
        }
        #endregion

        #region CreateProfileAsync and private methods
        private async Task<bool> CreateProfileAsync(IProfile profile, string avatarUrl)
        {
            switch (profile)
            {
                case AdminDto adminDto:
                    return await CreateAdminProfileAsync(adminDto, avatarUrl);
                case DentistDto dentistDto:
                    return await CreateDentistProfileAsync(dentistDto, avatarUrl);
                case PatientDto patientDto:
                    return await CreatePatientProfileAsync(patientDto, avatarUrl);
                case ReceptionistDto receptionistDto:
                    return await CreateReceptionistProfileAsync(receptionistDto, avatarUrl);
            }

            return false;
        }

        private async Task<bool> CreateAdminProfileAsync(AdminDto adminProfile, string avatarUrl)
        {
            adminProfile.LicenseType = LicenseType.Trial;
            adminProfile.LicenseStartDate = DateTime.Now;
            adminProfile.LicenseEndDate = DateTime.Now.AddDays(10);
            adminProfile.ProfileImageUrl = avatarUrl;

            return await _adminRepository.CreateAsync(adminProfile);
        }

        private async Task<bool> CreateDentistProfileAsync(DentistDto dentistProfile, string avatarUrl)
        {
            dentistProfile.ProfileImageUrl = avatarUrl;

            return await _dentistRepository.CreateAsync(dentistProfile);
        }

        private async Task<bool> CreatePatientProfileAsync(PatientDto patientProfile, string avatarUrl)
        {
            patientProfile.ProfileImageUrl = avatarUrl;

            return await _patientRepository.CreateAsync(patientProfile);
        }

        private async Task<bool> CreateReceptionistProfileAsync(ReceptionistDto receptionistProfile, string avatarUrl)
        {
            receptionistProfile.ProfileImageUrl = avatarUrl;

            return await _dentistOfficeRepository.CreateRecepcionistAsync(receptionistProfile);
        }
        #endregion

        #region UpdateProfileAsync and private methods
        private async Task<bool> UpdateProfileAsync(IProfile profile, string avatarUrl)
        {
            switch (profile)
            {
                case AdminDto adminDto:
                    return await UpdateAdminProfileAsync(adminDto, avatarUrl);
                case DentistDto dentistDto:
                    return await UpdateDentistProfileAsync(dentistDto, avatarUrl);
                case PatientDto patientDto:
                    return await UpdatePatientProfileAsync(patientDto, avatarUrl);
                case ReceptionistDto receptionistDto:
                    return await UpdateReceptionistProfileAsync(receptionistDto, avatarUrl);
            }

            return false;
        }

        private async Task<bool> UpdateAdminProfileAsync(AdminDto adminDto, string avatarUrl)
        {
            adminDto.ProfileImageUrl = avatarUrl;

            return await _adminRepository.UpdateAsync(adminDto);
        }

        private async Task<bool> UpdateDentistProfileAsync(DentistDto dentistDto, string avatarUrl)
        {
            dentistDto.ProfileImageUrl = avatarUrl;

            return await _dentistRepository.UpdateAsync(dentistDto);
        }

        private async Task<bool> UpdatePatientProfileAsync(PatientDto patientDto, string avatarUrl)
        {
            patientDto.ProfileImageUrl = avatarUrl;

            return await _patientRepository.UpdateAsync(patientDto);
        }

        private async Task<bool> UpdateReceptionistProfileAsync(ReceptionistDto receptionistDto, string avatarUrl)
        {
            receptionistDto.ProfileImageUrl = avatarUrl;

            return await _receptionistRepository.UpdateAsync(receptionistDto);
        }
        #endregion

        #region RemoveProfileAsync and private methods
        private async Task RemoveProfileAsync(IProfile profile)
        {
            switch (profile)
            {
                case AdminDto adminDto:
                    await RemoveAdminProfileAsync(adminDto);
                    break;
                case DentistDto dentistDto:
                    await RemoveDentistProfile(dentistDto);
                    break;
                case PatientDto patientDto:
                    await RemovePatientProfileAsync(patientDto);
                    break;
                case ReceptionistDto receptionistDto:
                    await RemoveReceptionistProfile(receptionistDto);
                    break;
            }
        }

        private async Task RemoveAdminProfileAsync(AdminDto adminDto)
        {
            await _adminRepository.RemoveAsync(adminDto);
        }

        private async Task RemoveReceptionistProfile(ReceptionistDto receptionistDto)
        {
            await _dentistOfficeRepository.RemoveRecepcionistAsync(receptionistDto);
        }

        private async Task RemovePatientProfileAsync(PatientDto patientDto)
        {
            await _patientRepository.RemoveAsync(patientDto);
        }

        private async Task RemoveDentistProfile(DentistDto dentistDto)
        {
            await _dentistRepository.RemoveAsync(dentistDto);
        }
        #endregion

        #endregion
    }
}