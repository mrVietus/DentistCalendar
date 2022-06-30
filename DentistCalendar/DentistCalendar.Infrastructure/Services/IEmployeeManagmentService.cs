using DentistCalendar.Dto.DTO.Domain;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services
{
    public interface IEmployeeManagmentService : IService
    {
        Task<IEnumerable<DentistOfficeDto>> GetEmployeesAsync(string profileId);
        Task<object> TryCreateRecepcionistAccountWithProfile(string email, string temporaryPassword, int dentistOfficeId, string confirmLink, string successLink);
        Task<bool> RemoveRecepcionistAsync(int id);
        Task<object> TryInviteDentistAsync(string emailToInvite, string invitingProfileId, int dentistOfficeId, string acceptInvitationLink, string successLink);
        Task<bool> RemoveDentistInvitationAsync(string email);
        Task<bool> RemoveDentistAsync(int dentistId, int dentistOfficeId);
    }
}