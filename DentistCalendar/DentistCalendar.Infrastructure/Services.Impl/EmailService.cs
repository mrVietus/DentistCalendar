using DentistCalendar.Common.Extensions;
using DentistCalendar.Common.Logger;
using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Infrastructure.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services.Impl
{
    public class EmailService : IEmailService
    {
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;
        private readonly ILoggerService _loggerService;

        public EmailService(EmailSettings emailSettings, ILoggerService loggerService)
        {
            _loggerService = loggerService;
            _host = emailSettings.Host;
            _username = emailSettings.Username;
            _password = emailSettings.Password;
        }

        public Task TrySendEmail(SendEmailDto emailDto)
        {
            _loggerService.Info($"Trying to send email to: {string.Join(",", emailDto.To)}.");
            if (!ValidateDto(emailDto))
            {
                _loggerService.Error($"Sending email to: {string.Join(",", emailDto.To)} failed. Dto validation failed.");
            }

            SendEmailAsync(emailDto.From, emailDto.To, emailDto.Subject, emailDto.Body, emailDto.IsBodyHtml);

            return Task.CompletedTask;
        }

        private bool ValidateDto(SendEmailDto emailDto)
        {
            if (!emailDto.To.Any()) return false;
            if (emailDto.From.IsNullOrEmpty()) return false;
            if (emailDto.Body.IsNullOrEmpty()) return false;
            if (emailDto.Subject.IsNullOrEmpty()) return false;

            return true;
        }

        private void SendEmailAsync(string from, ICollection<string> to, string subject, string body, bool isBodyHtml)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(from),
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };

            foreach (var email in to)
            {
                mailMessage.To.Add(new MailAddress(email));
            }

            var smtp = new SmtpClient()
            {
                Host = _host,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_username, _password),
            };
            smtp.SendCompleted += new SendCompletedEventHandler(SmtpClient_OnCompleted);
            smtp.SendCompleted += (s, e) => mailMessage.Dispose();

            try
            {
                var messageGuid = Guid.NewGuid();
                _loggerService.Info($"Sending email '{subject}' to {string.Join(",", mailMessage.To.Select(e => e.Address))}. Message token: {messageGuid}");
                smtp.SendAsync(mailMessage, messageGuid.ToString());
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Sending email to: {to} failed.", ex);
            }
        }

        private void SmtpClient_OnCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _loggerService.Info($"Sending email for Message token: {e.UserState} was canceled.");
            }
            if (e.Error != null)
            {
                _loggerService.Error($"Sending email for Message token: {e.UserState} failed. Error: {e.Error.ToString()}");
            }
            else
            {
                _loggerService.Info($"Sending email for Message token: {e.UserState} succeed.");
            }
        }
    }
}