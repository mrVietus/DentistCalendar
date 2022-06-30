using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Infrastructure.Commands;
using DentistCalendar.Infrastructure.Commands.Impl.Email;
using DentistCalendar.Infrastructure.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Handlers.Email
{
    public class SendConfirmRegistrationEmailHandler : ICommandHandler<SendConfirmRegistrationEmail>
    {
        private readonly IEmailService _emailService;

        public SendConfirmRegistrationEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public async Task HandleAsync(SendConfirmRegistrationEmail command)
        {
            var emailDto = new SendEmailDto
            {
                Body = command.Body,
                From = command.From,
                IsBodyHtml = false,
                Subject = command.Subject,
                To = new List<string> { command.To }
            };

            await _emailService.TrySendEmail(emailDto);
        }
    }
}