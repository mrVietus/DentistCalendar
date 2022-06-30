using AutoMapper;
using DentistCalendar.Common.Logger;
using DentistCalendar.Core.Entities;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories.Impl
{
    public class ReceptionistRepository : IReceptionistRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly DentistCalendarDbContext _dbContext;

        public ReceptionistRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<ReceptionistDto> GetByIdAsync(int id)
        {
            try
            {
                var receptionistEntity = _dbContext.Receptionists.Include(x => x.DentistOffice).FirstOrDefault(x => x.Id == id);
                if (receptionistEntity == null) return await Task.FromResult<ReceptionistDto>(null);

                return await Task.FromResult(_mapper.Map<Receptionist, ReceptionistDto>(receptionistEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting recepcionist by id {id} failed.", ex);
                return await Task.FromResult<ReceptionistDto>(null);
            }
        }

        public async Task<ReceptionistDto> GetByProfileIdAsync(Guid id)
        {
            try
            {
                var receptionistEntity = _dbContext.Receptionists.FirstOrDefault(x => x.ProfileId == id);
                if (receptionistEntity == null) return await Task.FromResult<ReceptionistDto>(null);

                return await Task.FromResult(_mapper.Map<Receptionist, ReceptionistDto>(receptionistEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting recepcionist by profile id {id} failed.", ex);
                return await Task.FromResult<ReceptionistDto>(null);
            }
        }

        public async Task<bool> RemoveAsync(ReceptionistDto receptionist)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var receptionistEntity = _dbContext.Receptionists.FirstOrDefault(x => x.ProfileId == receptionist.ProfileId);
                    if (receptionistEntity == null) return await Task.FromResult(true);

                    _dbContext.Receptionists.Remove(receptionistEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove recepcionist with id {receptionist.Id} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> UpdateAsync(ReceptionistDto receptionist)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var receptionistEntity = _dbContext.Receptionists.FirstOrDefault(x => x.ProfileId == receptionist.ProfileId);
                    if (receptionistEntity == null) return await Task.FromResult(false);

                    receptionistEntity.Update(_mapper.Map<ReceptionistDto, Receptionist>(receptionist));

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update recepcionist with id {receptionist.Id} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }
    }
}