using DentistCalendar.Common.Logger;
using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Infrastructure.Commands;
using DentistCalendar.Infrastructure.Commands.Impl.Email;
using DentistCalendar.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IEncrypter _encrypter;
        private readonly ILoggerService _loggerService;
        private readonly IUserRepository _userRepository;

        public UserService(ICommandDispatcher commandDispatcher, IEncrypter encrypter, ILoggerService loggerService, IUserRepository userRepository)
        {
            _commandDispatcher = commandDispatcher;
            _encrypter = encrypter;
            _loggerService = loggerService;
            _userRepository = userRepository;
        }

        public async Task<AuthenticationDto> AuthenticateAsync(string email, string password)
        {
            var clearEmail = ClearEmail(email);
            _loggerService.Info($"Authenticating user with email: {clearEmail}");
            var user = await _userRepository.GetByEmail(clearEmail);
            if (user == null) return null;

            if (!ValidateUserPassword(password, user)) return null;

            return CreateAuthenticationDtoForUser(user);
        }

        public async Task<bool> TryRegisterAsync(RegistrationDto registrationDto)
        {
            var clearEmail = ClearEmail(registrationDto.Email);
            _loggerService.Info($"Trying register user with email: {clearEmail}");
            var user = await _userRepository.GetByEmail(clearEmail);

            if (user != null) return false;
            _loggerService.Info($"Creating user with email: {clearEmail}");
            user = CreateUser(registrationDto, clearEmail);

            if (user == null) return false;

            if (!await _userRepository.AddAsync(user)) return false;
            _loggerService.Info($"Wser with email: {clearEmail} was added to DB");

            var confirmationLink = CreateConfirmationLink(registrationDto.RegistrationConfirmationLink, user.Id, clearEmail);
            SendConfirmRegistrationEmail(confirmationLink, clearEmail);

            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string registrationGuid, string email)
        {
            var clearEmail = ClearEmail(email);
            var user = await _userRepository.GetByEmail(clearEmail);

            if (user.Id.ToString() != registrationGuid)
            {
                return false;
            }

            return await _userRepository.ActivateUserAsync(clearEmail);
        }

        public async Task<bool> UnregisterAsync(string email, string password, bool force)
        {
            if (force)
            {
                return await _userRepository.RemoveAsync(email);
            }

            var user = await _userRepository.GetByEmail(email);
            if (!ValidateUserPassword(password, user)) return false;

            return await _userRepository.RemoveAsync(email);
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByEmail(changePasswordDto.Email);
            if (user == null) return false;

            if (!ValidateUserPassword(changePasswordDto.OldPassword, user)) return false;
            return await UpdatePassword(changePasswordDto);
        }

        public async Task<UserDto> GetUserDataByEmailAsync(string email)
        {
            var clearEmail = ClearEmail(email);
            _loggerService.Info($"Getting user data for email: {clearEmail}");
            var user = await _userRepository.GetByEmail(clearEmail);
            if (user == null) return null;

            return user;
        }

        public async Task<UserDto> GetUserDataByProfileIdAsync(Guid profileId)
        {
            _loggerService.Info($"Getting user data for profileId: {profileId}");
            var user = await _userRepository.GetByProfileIdAsync(profileId);
            if (user == null) return null;

            return user;
        }

        private bool ValidateUserPassword(string password, UserDto user)
        {
            try
            {
                var hash = _encrypter.GetHash(password, user.Salt);

                if (user.Password != hash) return false;

                return true;
            }
            catch (ArgumentException ae)
            {
                _loggerService.Error($"Authenticating user with email: {user.Email} failed. GetHash throw exception: {ae.Message}", ae);
                return false;
            }
        }

        private AuthenticationDto CreateAuthenticationDtoForUser(UserDto user)
        {
            var authDto = new AuthenticationDto
            {
                IsActive = user.IsActive
            };

            if (!user.IsActive) return authDto;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Email),
                new Claim(ClaimTypes.Role, user.AccountType.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            if (user.ProfileId != Guid.Empty)
            {
                claims.Add(new Claim(ClaimTypes.UserData, user.ProfileId.ToString()));
            }

            authDto.ClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
            authDto.AccountType = user.AccountType;

            return authDto;
        }

        private UserDto CreateUser(RegistrationDto registrationDto, string email)
        {
            try
            {
                var salt = _encrypter.GetSalt(registrationDto.Password);
                var hash = _encrypter.GetHash(registrationDto.Password, salt);

                return new UserDto
                {
                    Id = Guid.NewGuid(),
                    AccountType = registrationDto.AccountType,
                    Email = email,
                    Password = hash,
                    Salt = salt
                };
            }
            catch (ArgumentException ae)
            {
                _loggerService.Error($"Registering user with email: {email} failed. GetHash throw exception: {ae.Message}", ae);
                return null;
            }
        }

        private async Task<bool> UpdatePassword(ChangePasswordDto changePasswordDto)
        {
            var salt = _encrypter.GetSalt(changePasswordDto.NewPassword);
            var hash = _encrypter.GetHash(changePasswordDto.NewPassword, salt);

            return await _userRepository.UpdatePasswordAsync(changePasswordDto.Email, hash, salt);
        }

        private void SendConfirmRegistrationEmail(string link, string clearEmail)
        {
            var sendConfirmRegistrationEmail = new SendConfirmRegistrationEmail
            {
                Body = $"Witamy w aplikacji dentist calendar. Dziękujemy blah blah {Environment.NewLine} Link aktywacyjny: {link}",
                From = "sevarius7@gmail.com",
                Subject = "Witamy w DentistCalendar! Potwierdź swoją rejestrację.",
                To = clearEmail
            };

            _commandDispatcher.DispatchAsync(sendConfirmRegistrationEmail);
        }

        private string CreateConfirmationLink(string link, Guid userId, string email)
        {
            _loggerService.Info($"Creating confirmation link for email {email}");
            return link.Replace("HereShouldBeGuid", userId.ToString()).Replace("HereShouldBeEmail", email);
        }

        private string ClearEmail(string email)
        {
            return email.Trim().ToLowerInvariant();
        }
    }
}