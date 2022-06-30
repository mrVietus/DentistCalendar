using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories
{
    public interface IInvitationRepository : IRepository
    {
        Task<bool> CreateAsync(InvitationDto invitationDto);
        Task<InvitationDto> GetAsync(string invitationGuid);
        Task<bool> RemoveAsync(string email);
        Task<IEnumerable<InvitationDto>> GetForProfileAsync(string profileId);
    }
}