using DentistCalendar.Common.Enums;
using DentistCalendar.Common.Logger;
using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace DentistCalendar.Infrastructure.Services.Impl
{
    public class EmployeeManagmentService : IEmployeeManagmentService
    {
        private readonly IDentistOfficeRepository _dentistOfficeRepository;
        private readonly IInvitationService _invitationService;
        private readonly ILoggerService _loggerService;
        private readonly ILicenseOwnerService _licenseOwnerService;
        private readonly IProfileService _profileService;
        private readonly IReceptionistRepository _receptionistRepository;
        private readonly IUserService _userService;

        public EmployeeManagmentService(IDentistOfficeRepository dentistOfficeRepository, IInvitationService invitationService, ILoggerService loggerService,
            ILicenseOwnerService LicenseOwnerService, IProfileService profileService, IReceptionistRepository receptionistRepository, IUserService userService)
        {
            _dentistOfficeRepository = dentistOfficeRepository;
            _invitationService = invitationService;
            _loggerService = loggerService;
            _licenseOwnerService = LicenseOwnerService;
            _profileService = profileService;
            _receptionistRepository = receptionistRepository;
            _userService = userService;
        }

        public async Task<IEnumerable<DentistOfficeDto>> GetEmployeesAsync(string profileId)
        {
            var dentistOffices = await _licenseOwnerService.GetDentistOfficesAsync(profileId);

            if (dentistOffices != null)
            {
                var dentistOfficesWithFilledData = new List<DentistOfficeDto>();

                foreach (var dentistOffice in dentistOffices)
                {
                    dentistOfficesWithFilledData.Add(await _dentistOfficeRepository.GetByIWithRecepcionistAndDentistAsync(dentistOffice.Id));
                }

                //fill email
                foreach(var dentistOffice in dentistOfficesWithFilledData)
                {
                    foreach(var dentist in dentistOffice.Dentists)
                    {
                        var profile = await _userService.GetUserDataByProfileIdAsync(dentist.ProfileId);

                        dentist.Email = profile.Email;
                    }
                }

                return dentistOfficesWithFilledData;
            }

            return null;
        }

        public async Task<object> TryCreateRecepcionistAccountWithProfile(string email, string temporaryPassword, int dentistOfficeId, string confirmLink, string successLink)
        {
            var registration = new RegistrationDto
            {
                AccountType = AccountType.Receptionist,
                Email = email,
                Password = temporaryPassword,
                RepeatPassword = temporaryPassword,
                RegistrationConfirmationLink = confirmLink
            };

            if (!await _userService.TryRegisterAsync(registration))
            {
                return new { status = "error", modelErrors = $"[\"Urzytkownik o adresie o adresie e-mail: {email} już istnieje.\"]" };
            }

            var receptionistDto = new ReceptionistDto
            {
                Name = "",
                LastName = "",
                MobilePhone = "",
                Email = email,
                DentistOffice = await _dentistOfficeRepository.GetByIdAsync(dentistOfficeId)
            };

            var profileId = await _profileService.TryCreateProfileAsync(receptionistDto, null);

            if (string.IsNullOrEmpty(profileId))
            {
                await _userService.UnregisterAsync(email, "", true);

                return new { status = "error", modelErrors = $"[\"Wystąpił nieoczekiwany błąd podczas tworzenia profilu.\"]" };
            }

            return new { status = "success", url = successLink };
        }

        public async Task<bool> RemoveRecepcionistAsync(int id)
        {
            var recepcionist = await _receptionistRepository.GetByIdAsync(id);

            if (recepcionist == null)
            {
                return false;
            }

            if (!await _userService.UnregisterAsync(recepcionist.Email, "", true))
            {
                return false;
            }

            if (!await _dentistOfficeRepository.RemoveRecepcionistAsync(recepcionist))
            {
                return false;
            }

            return await _receptionistRepository.RemoveAsync(recepcionist);
        }

        public async Task<object> TryInviteDentistAsync(string emailToInvite, string invitingProfileId, int dentistOfficeId, string acceptInvitationLink, string successLink)
        {
            if (string.IsNullOrEmpty(acceptInvitationLink)
                || string.IsNullOrEmpty(successLink)
                || string.IsNullOrEmpty(invitingProfileId)
                || string.IsNullOrEmpty(emailToInvite))
            {
                return new { status = "error", modelErrors = $"[\"Wystąpił nieoczekiwany błąd podczas wysyłania zaproszenia.\"]" };
            }

            var userData = await _userService.GetUserDataByEmailAsync(emailToInvite);

            if (userData == null || userData.AccountType != AccountType.Dentist)
            {
                return new { status = "error", modelErrors = $"[\"Użytkownik o mailu: {emailToInvite} nie istnieje lub nie posiada konta dentysty. Zaproszenie nie zostało wysłane.\"]" };
            }

            if (userData.ProfileId == Guid.Empty)
            {
                return new { status = "error", modelErrors = $"[\"Użytkownik o mailu: {emailToInvite} nie nie posiada stworzonego profilu. Zaproszenie nie zostało wysłane.\"]" };
            }

            var dentistOffice = await _dentistOfficeRepository.GetByIdAsync(dentistOfficeId);

            if (dentistOffice == null)
            {
                return new { status = "error", modelErrors = $"[\"Coś poszło nie tak podczas pobierania danych o Twoim gabinecie. Zaproszenie nie zostało wysłane.\"]" };
            }

            var invitationGuid = Guid.NewGuid();
            var invitationDto = new SendInvitationDto()
            {
                AcceptInvitationLink = CreateAcceptInvitationLink(acceptInvitationLink, invitationGuid),
                DentistOffice = dentistOffice,
                InvitationGuid = invitationGuid,
                InvitedAccountType = AccountType.Dentist,
                InvitedProfileId = userData.ProfileId.ToString(),
                InvitedEmail = userData.Email,
                InvitingAccountType = AccountType.Admin,
                InvitingProfileId = invitingProfileId
            };

            if (!await _invitationService.TrySendInvitationAsync(invitationDto))
            {
                return new { status = "error", modelErrors = $"[\"Wystąpił nieoczekiwany błąd podczas wysyłania zaproszenia.\"]" };
            }

            return new { status = "success", url = successLink };
        }

        public async Task<bool> RemoveDentistInvitationAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return await _invitationService.RemoveInvitationAsync(email);
        }

        public async Task<bool> RemoveDentistAsync(int dentistId, int dentistOfficeId)
        {
            return await _dentistOfficeRepository.RemoveDentistAsync(dentistId, dentistOfficeId);
        }

        private string CreateAcceptInvitationLink(string link, Guid invitationLinkGuid)
        {
            return link.Replace("HereShouldBeGuid", invitationLinkGuid.ToString());
        }
    }
}