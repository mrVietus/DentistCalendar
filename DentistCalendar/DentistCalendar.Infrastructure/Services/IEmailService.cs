using DentistCalendar.Dto.DTO.Application;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services
{
    public interface IEmailService : IService
    {
        Task TrySendEmail(SendEmailDto emailDto);
    }
}