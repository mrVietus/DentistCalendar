using DentistCalendar.Dto.DTO.Application;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services
{
    public interface IInvitationService : IService
    {
        Task<bool> TrySendInvitationAsync(SendInvitationDto invitationDto);
        Task<object> TryAcceptInvitationAsync(string invitationId, string successLink);
        Task<bool> TryAcceptInvitationFromLinkAsync(string invitationId, string email);
        Task<IEnumerable<InvitationDataDto>> GetInvitationsForProfileAsync(string profileId);
        Task<object> RejectInvitationAsync(string invitationId, string successLink);
        Task<bool> RemoveInvitationAsync(string email);
    }
}