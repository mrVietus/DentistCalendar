using AutoMapper;
using DentistCalendar.Common.Logger;
using DentistCalendar.Core.Entities;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Persistence;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories.Impl
{
    public class UserRepository : IUserRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly DentistCalendarDbContext _dbContext;
        private readonly IMapper _mapper;

        public UserRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(UserDto user)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userEntity = new User
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Password = user.Password,
                        Salt = user.Salt,
                        AccountType = user.AccountType,
                        IsActive = false
                    };

                    await _dbContext.Users.AddAsync(userEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of user with email {user.Email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public Task<bool> AddProfileId(string email, Guid profileId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userEntity = _dbContext.Users.FirstOrDefault(x => x.Email == email);
                    if (userEntity == null) return Task.FromResult(false);

                    userEntity.ProfileId = profileId;
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Adding to user profile id failed.", ex);
                    return Task.FromResult(false);
                }
            }
        }

        public async Task<UserDto> GetByProfileIdAsync(Guid profileId)
        {
            try
            {
                var userEntity = _dbContext.Users.FirstOrDefault(x => x.ProfileId == profileId);
                if (userEntity == null) return await Task.FromResult<UserDto>(null);

                return await Task.FromResult(_mapper.Map<User, UserDto>(userEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting user by profileId {profileId} failed.", ex);
                return await Task.FromResult<UserDto>(null);
            }
        }

        public Task<UserDto> GetByEmail(string email)
        {
            try
            {
                var userEntity = _dbContext.Users.FirstOrDefault(x => x.Email == email);
                if (userEntity == null) return Task.FromResult<UserDto>(null);

                return Task.FromResult(_mapper.Map<User, UserDto>(userEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting user by email {email} failed.", ex);
                return Task.FromResult<UserDto>(null);
            }
        }

        public async Task<bool> RemoveAsync(string email)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userEntity = _dbContext.Users.FirstOrDefault(x => x.Email == email);
                    if (userEntity == null) return await Task.FromResult(true);

                    _dbContext.Users.Remove(userEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove user with email {email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> UpdateAsync(string email, UserDto user)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userEntity = _dbContext.Users.FirstOrDefault(x => x.Email == email);
                    if (userEntity == null) return await Task.FromResult(false);

                    userEntity.Update(_mapper.Map<UserDto, User>(user));

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update user with email {email} and account type {user.AccountType} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> UpdatePasswordAsync(string email, string newPassword, string newSalt)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userEntity = _dbContext.Users.FirstOrDefault(x => x.Email == email);
                    if (userEntity == null) return await Task.FromResult(false);

                    userEntity.UpdatePassword(newPassword, newSalt);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update password for user with email {email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> ActivateUserAsync(string email)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var userEntity = _dbContext.Users.FirstOrDefault(x => x.Email == email);
                    if (userEntity == null) return await Task.FromResult(false);

                    userEntity.IsActive = true;

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Activate user with email {email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }
    }
}