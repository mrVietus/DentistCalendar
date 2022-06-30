using AutoMapper;
using AutoMapper.QueryableExtensions;
using DentistCalendar.Common.Logger;
using DentistCalendar.Core.Entities;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories.Impl
{
    public class InvitationRepository  : IInvitationRepository
    {
        private readonly DentistCalendarDbContext _dbContext;
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;

        public InvitationRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(InvitationDto invitation)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var invitationEntity = _dbContext.Invitations.FirstOrDefault(x => x.Id == invitation.Id);
                    if (invitationEntity != null) return await Task.FromResult(false);

                    invitationEntity = new Invitation
                    {
                        Guid = invitation.InvitationGuid.ToString(),
                        InvitedAccountType = invitation.InvitedAccountType,
                        InvitedEmail = invitation.InvitedEmail,
                        InvitedProfileId = invitation.InvitedProfileId,
                        InvitingProfileId = invitation.InvitingProfileId,
                        InvitingDentistOfficeId = invitation.InvitingDentistOfficeId,
                        InvitingName = invitation.InvitingName
                    };

                    await _dbContext.Invitations.AddAsync(invitationEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of invitation for email: {invitation.InvitedEmail} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<InvitationDto> GetAsync(string invitationGuid)
        {
            try
            {
                var invitationEntity = _dbContext.Invitations.FirstOrDefault(x => x.Guid == invitationGuid);
                if (invitationEntity == null) return await Task.FromResult<InvitationDto>(null);

                return await Task.FromResult(_mapper.Map<Invitation, InvitationDto>(invitationEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting invitation by guid {invitationGuid} failed.", ex);
                return await Task.FromResult<InvitationDto>(null);
            }
        }

        public async Task<IEnumerable<InvitationDto>> GetForProfileAsync(string profileId)
        {
            try
            {
                return await _dbContext.Invitations.Where(x => x.InvitedProfileId == profileId || x.InvitingProfileId == profileId)
                    .ProjectTo<InvitationDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Get invitations for profileId: {profileId} failed.", ex);
                return await Task.FromResult<IEnumerable<InvitationDto>>(null);
            }
        }

        public async Task<bool> RemoveAsync(string email)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var invitationEntity = _dbContext.Invitations.FirstOrDefault(x => x.InvitedEmail == email);
                    if (invitationEntity == null) return await Task.FromResult(true);

                    _dbContext.Invitations.Remove(invitationEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove invitation for invited email {email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }
    }
}