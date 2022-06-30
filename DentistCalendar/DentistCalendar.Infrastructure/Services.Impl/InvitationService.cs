using AutoMapper;
using DentistCalendar.Common.Enums;
using DentistCalendar.Common.Logger;
using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Commands;
using DentistCalendar.Infrastructure.Commands.Impl.Email;
using DentistCalendar.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services.Impl
{
    public class InvitationService : IInvitationService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IDentistRepository _dentistRepository;
        private readonly IDentistOfficeRepository _dentistOfficeRepository;
        private readonly IInvitationRepository _invitationRepository;
        private readonly IMapper _mapper;
        private readonly IProfileService _profileService;
        private readonly ILoggerService _loggerService;


        public InvitationService(IAdminRepository adminRepository, ICommandDispatcher commandDispatcher, IDentistRepository dentistRepository, IDentistOfficeRepository dentistOfficeRepository,
                                 IInvitationRepository invitationRepository, IMapper mapper,
                                 IProfileService profileService, ILoggerService loggerService)
        {
            _adminRepository = adminRepository;
            _commandDispatcher = commandDispatcher;
            _dentistRepository = dentistRepository;
            _dentistOfficeRepository = dentistOfficeRepository;
            _invitationRepository = invitationRepository;
            _mapper = mapper;
            _profileService = profileService;
            _loggerService = loggerService;
        }

        public async Task<bool> TrySendInvitationAsync(SendInvitationDto sendInvitationDto)
        {
            if (sendInvitationDto == null)
            {
                return false;
            }

            var invitationDto = _mapper.Map<SendInvitationDto, InvitationDto>(sendInvitationDto);
            var invitingProfile = await _profileService.GetProfileForUser(sendInvitationDto.InvitingProfileId, sendInvitationDto.InvitingAccountType);
            invitationDto.InvitingName = invitingProfile.Name;

            if (sendInvitationDto.DentistOffice != null)
            {
                invitationDto.InvitingDentistOfficeId = sendInvitationDto.DentistOffice.Id.ToString();
            }

            return !await _invitationRepository.CreateAsync(invitationDto)
                ? false
                : await PrepareAndSendInvitationEmail(sendInvitationDto, invitationDto, invitingProfile);
        }

        public async Task<IEnumerable<InvitationDataDto>> GetInvitationsForProfileAsync(string profileId)
        {
            if (string.IsNullOrEmpty(profileId))
            {
                return await Task.FromResult<IEnumerable<InvitationDataDto>>(null);
            }

            var invitations = await _invitationRepository.GetForProfileAsync(profileId);
            var invitationDataDtoList = _mapper.Map<IEnumerable<InvitationDto>, IEnumerable<InvitationDataDto>>(invitations);


            if (invitationDataDtoList.Any())
            {
                if (profileId == invitations.FirstOrDefault().InvitingProfileId)
                {
                    foreach (var invitationDataDto in invitationDataDtoList)
                    {
                        var dentist = await _dentistRepository.GetByProfileIdAsync(Guid.Parse(invitationDataDto.InvitedProfileId));

                        if (string.IsNullOrEmpty(dentist.DoctorTitle))
                        {
                            invitationDataDto.InvitorName = $"{dentist.Name} {dentist.LastName}";
                        }
                        else
                        {
                            invitationDataDto.InvitorName = $"{dentist.DoctorTitle} {dentist.Name} {dentist.LastName}";
                        }

                        invitationDataDto.DiseableAccept = true;
                    }
                }
                else
                {
                    var invitingOfficeIds = invitations.Select(x => x.InvitingDentistOfficeId).Distinct();

                    if (invitingOfficeIds.Any())
                    {
                        var dentistOfficesDtoList = new List<DentistOfficeDto>();

                        foreach (var dentistOfficeId in invitingOfficeIds)
                        {
                            var id = int.Parse(dentistOfficeId);
                            dentistOfficesDtoList.Add(await _dentistOfficeRepository.GetByIdAsync(id));
                        }

                        foreach (var invitationDataDto in invitationDataDtoList)
                        {
                            invitationDataDto.InvitorName = dentistOfficesDtoList.FirstOrDefault(x => x.Id == invitationDataDto.InvitingDentistOfficeId).Name;
                            invitationDataDto.DiseableAccept = false;
                        }
                    }
                }
            }

            return invitationDataDtoList;
        }

        public async Task<object> TryAcceptInvitationAsync(string invitationId, string successLink)
        {
            if (string.IsNullOrEmpty(invitationId))
            {
                return new { status = "error", modelErrors = $"[\"Błędne id zaproszenia.\"]" };
            }

            if (invitationId == Guid.Empty.ToString())
            {
                return new { status = "error", modelErrors = $"[\"Błędne id zaproszenia.\"]" };
            }

            if (!await AcceptInvitationAsync(invitationId))
            {
                return new { status = "error", modelErrors = $"[\"Błęd podczas akceptacji zaproszenia.\"]" };
            }

            return new { status = "success", url = successLink };
        }

        public async Task<bool> TryAcceptInvitationFromLinkAsync(string invitationId, string email)
        {
            if (string.IsNullOrEmpty(invitationId) || string.IsNullOrEmpty(email))
            {
                return false;
            }

            return await AcceptInvitationAsync(invitationId, email);
        }

        public async Task<object> RejectInvitationAsync(string invitationId, string successLink)
        {
            if (string.IsNullOrEmpty(invitationId))
            {
                return new { status = "error", modelErrors = $"[\"Błędne id zaproszenia.\"]" };
            }

            if (invitationId == Guid.Empty.ToString())
            {
                return new { status = "error", modelErrors = $"[\"Błędne id zaproszenia.\"]" };
            }

            if (!await RejectInvitationAsync(invitationId))
            {
                return new { status = "error", modelErrors = $"[\"Błęd podczas odrzucenia zaproszenia.\"]" };
            }

            return new { status = "success", url = successLink };
        }

        public async Task<bool> RemoveInvitationAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            return await _invitationRepository.RemoveAsync(email);
        }

        #region PrivateMethods

        private async Task<bool> PrepareAndSendInvitationEmail(SendInvitationDto sendInvitationDto, InvitationDto invitationDto, IProfile invitingProfile)
        {
            try
            {
                var invitedProfile = await _profileService.GetProfileForUser(sendInvitationDto.InvitedProfileId, sendInvitationDto.InvitedAccountType);
                var body = "";
                var subject = "";

                //ToDo: care about other invitation types
                if (sendInvitationDto.InvitingAccountType == AccountType.Admin && sendInvitationDto.InvitedAccountType == AccountType.Dentist)
                {
                    subject = $"DentistCalendar - Prośba o dołączenie do {sendInvitationDto.DentistOffice.Name}";
                    body = $"{invitingProfile.Name} Pragnie zaprosić Pana {invitedProfile.Name} do dołączenia do placówki dentystycznej {sendInvitationDto.DentistOffice.Name} {Environment.NewLine}" +
                        $"jako dentysta. Aby zaakceptować zaproszenie i dołączyć do wyżej wymienionej placówki dentystycznej proszę zalogować się na swoje konto oraz w karcie zaproszenia {Environment.NewLine}" +
                        $"zaakceptować zaproszenie od {sendInvitationDto.DentistOffice.Name} lub kliknąć w link: {sendInvitationDto.AcceptInvitationLink} .";
                }

                SendEmail(invitationDto.InvitedEmail, subject, body);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> PrepareAndSendRejectInvitationEmail(InvitationDto invitationDto)
        {
            try
            {
                //only dentist can reject Invitation of admins ( license owners ) invitation.
                var invitedProfile = await _dentistRepository.GetByProfileIdAsync(Guid.Parse(invitationDto.InvitedProfileId));
                var invitingProfile = await _adminRepository.GetByProfileIdWithDentistOfficesAsync(Guid.Parse(invitationDto.InvitingProfileId));

                if (invitedProfile == null || invitingProfile == null)
                {
                    return false;
                }

                var dentistOfficeName = invitingProfile.DentistOffices.FirstOrDefault(x => x.Id == int.Parse(invitationDto.InvitingDentistOfficeId)).Name;

                var subject = $"DentistCalendar - Prośba o dołączenie do {dentistOfficeName} przez {invitedProfile.DoctorTitle} {invitedProfile.Name} {invitedProfile.LastName} została odrzucona.";
                var bodyInvited = $"Potwierdzamy odrzucenie zaproszenia do ...";
                var bodyInviting = $"Dane z profilu dentysty - odrzucił Twoją prośbę dodania do Twojej placówki.";

                SendEmail(invitationDto.InvitedEmail, subject, bodyInvited);
                SendEmail(invitingProfile.Email, subject, bodyInviting);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> PrepareAndSendAcceptedInvitationEmail(InvitationDto invitationDto, DentistDto dentist, int dentistOfficeId)
        {
            try
            {
                var invitingProfile = await _adminRepository.GetByProfileIdWithDentistOfficesAsync(Guid.Parse(invitationDto.InvitingProfileId));

                var dentistOfficeName = invitingProfile.DentistOffices.FirstOrDefault(x => x.Id == dentistOfficeId).Name;

                var subject = $"DentistCalendar - Prośba o dołączenie do {dentistOfficeName} przez {dentist.DoctorTitle} {dentist.Name} {dentist.LastName} została zaakceptowana.";
                var bodyInvited = $"Potwierdzamy zaakceptowanie zaproszenia do ...";
                var bodyInviting = $"Dane z profilu dentysty - zaakceptował Twoją prośbę dołączenia do Twojej placówki.";

                SendEmail(invitationDto.InvitedEmail, subject, bodyInvited);
                SendEmail(invitingProfile.Email, subject, bodyInviting);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        private void SendEmail(string to, string subject, string body)
        {
            var sendConfirmRegistrationEmail = new SendConfirmRegistrationEmail
            {
                Body = body,
                From = "sevarius7@gmail.com",
                Subject = subject,
                To = to
            };

            _commandDispatcher.DispatchAsync(sendConfirmRegistrationEmail);
        }

        private async Task<bool> AcceptInvitationAsync(string invitationId, string email = "")
        {
            var invitation = await _invitationRepository.GetAsync(invitationId);

            if (!string.IsNullOrEmpty(email))
            {
                if (invitation.InvitedEmail != email)
                {
                    return false;
                }
            }

            bool succeded;
            switch (invitation.InvitedAccountType)
            {
                case AccountType.Admin:
                    succeded = HandleAdminInvitation(invitation);
                    break;
                case AccountType.Dentist:
                    succeded = await HandleDentistInvitationAsync(invitation);
                    break;
                case AccountType.Patient:
                    succeded = HandlePatientInvitation(invitation);
                    break;
                default:
                    return false;
            }

            return succeded ? await _invitationRepository.RemoveAsync(invitation.InvitedEmail) : false;
        }

        private bool HandlePatientInvitation(InvitationDto invitation)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> HandleDentistInvitationAsync(InvitationDto invitation)
        {
            var guid = Guid.Parse(invitation.InvitedProfileId);
            var dentist = await _dentistRepository.GetByProfileIdAsync(guid);

            if (dentist == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(invitation.InvitingDentistOfficeId))
            {
                return false;
            }

            var dentistOfficeId = int.Parse(invitation.InvitingDentistOfficeId);
            if (await _dentistOfficeRepository.AddDentistAsync(dentist.Id, dentistOfficeId))
            {
                return await PrepareAndSendAcceptedInvitationEmail(invitation, dentist, dentistOfficeId);
            }

            return false;
        }

        private bool HandleAdminInvitation(InvitationDto invitation)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> RejectInvitationAsync(string invitationId)
        {
            var invitation = await _invitationRepository.GetAsync(invitationId);

            if (!await PrepareAndSendRejectInvitationEmail(invitation))
            {
                return false;
            }

            return await _invitationRepository.RemoveAsync(invitation.InvitedEmail);
        }

        #endregion
    }
}