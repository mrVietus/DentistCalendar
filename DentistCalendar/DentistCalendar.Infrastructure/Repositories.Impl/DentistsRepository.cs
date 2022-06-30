using AutoMapper;
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
    public class DentistsRepository : IDentistRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly DentistCalendarDbContext _dbContext;

        public DentistsRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(DentistDto dentist)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistEntity = _dbContext.Dentists.FirstOrDefault(x => x.Id == dentist.Id);
                    if (dentistEntity != null) return await Task.FromResult(false);

                    dentistEntity = new Dentist
                    {
                        Adress = dentist.Adress,
                        Description = dentist.Description,
                        DoctorTitle = dentist.DoctorTitle,
                        LastName = dentist.LastName,
                        Name = dentist.Name,
                        MobilePhone = dentist.MobilePhone,
                        ProfileImageUrl = dentist.ProfileImageUrl,
                        ProfileId = dentist.ProfileId
                    };

                    await _dbContext.Dentists.AddAsync(dentistEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of dentist for email {dentist.Email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<DentistDto> GetByIdAsync(int id)
        {
            try
            {
                var dentistEntity = _dbContext.Dentists.FirstOrDefault(x => x.Id == id);
                if (dentistEntity == null) return await Task.FromResult<DentistDto>(null);

                return await Task.FromResult(_mapper.Map<Dentist, DentistDto>(dentistEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist by id {id} failed.", ex);
                return await Task.FromResult<DentistDto>(null);
            }
        }

        public async Task<IEnumerable<DentistDto>> GetWithServiceAndDentistOfficeAsync(int serviceId, int dentistOfficeId)
        {
            try
            {
                var dentists = _dbContext.Dentists
                    .Include(x => x.DentistDentistOffices)
                    .Include(x => x.DentistServices)
                    .Where(x => x.DentistDentistOffices.Any(y => y.DentistOfficeId == dentistOfficeId) &&
                                x.DentistServices.Any(z => z.ServiceId == serviceId))
                    .Select(o => new DentistDto
                    {
                        Id = o.Id,
                        Name = o.Name,
                        DoctorTitle = o.DoctorTitle,
                        LastName = o.LastName,
                        ProfileImageUrl = o.ProfileImageUrl
                    })
                    .ToList();

                if (dentists == null || !dentists.Any()) return Enumerable.Empty<DentistDto>();
                return await Task.FromResult(dentists);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Add proper info", ex);
                return Enumerable.Empty<DentistDto>();
            }
        }

        public async Task<DentistDto> GetByProfileIdAsync(Guid id)
        {
            try
            {
                var dentistEntity = _dbContext.Dentists.FirstOrDefault(x => x.ProfileId == id);
                if (dentistEntity == null) return await Task.FromResult<DentistDto>(null);

                return await Task.FromResult(_mapper.Map<Dentist, DentistDto>(dentistEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist by profile id {id} failed.", ex);
                return await Task.FromResult<DentistDto>(null);
            }
        }

        public async Task<bool> RemoveAsync(DentistDto dentist)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistEntity = _dbContext.Dentists.FirstOrDefault(x => x.Id == dentist.Id);
                    if (dentistEntity == null) return await Task.FromResult(true);

                    _dbContext.Dentists.Remove(dentistEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove dentist with id {dentist.Id} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> UpdateAsync(DentistDto dentist)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistEntity = _dbContext.Dentists.FirstOrDefault(x => x.ProfileId == dentist.ProfileId);
                    if (dentistEntity == null) return await Task.FromResult(false);

                    dentistEntity.Update(_mapper.Map<DentistDto, Dentist>(dentist));

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update dentist with id {dentist.Id} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }
    }
}