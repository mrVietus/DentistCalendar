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
    public class DentistOfficeRepository : IDentistOfficeRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly DentistCalendarDbContext _dbContext;

        public DentistOfficeRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<DentistOfficeDto> GetByIdAsync(int id)
        {
            try
            {
                var dentistOfficeEntity = _dbContext.DentistOffices.FirstOrDefault(x => x.Id == id);
                if (dentistOfficeEntity == null) return await Task.FromResult<DentistOfficeDto>(null);

                return await Task.FromResult(_mapper.Map<DentistOffice, DentistOfficeDto>(dentistOfficeEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist office by id {id} failed.", ex);
                return await Task.FromResult<DentistOfficeDto>(null);
            }
        }

        public async Task<DentistOfficeDto> GetWithServicesByIdAsync(int id)
        {
            try
            {
                var dentistOfficeEntity = _dbContext.DentistOffices.Select(o => new DentistOfficeDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Services = o.Services.Select(ser => new ServiceDto {
                        Id = ser.Id,
                        Dentists = ser.DentistServices.Select(x => new DentistDto { Name = x.Dentist.Name, LastName = x.Dentist.LastName, Id = x.Dentist.Id, DoctorTitle = x.Dentist.DoctorTitle }).ToList(),
                        Description = ser.Description,
                        Name = ser.Name,
                        Price = ser.Price,
                        Time = ser.Time
                    }).ToList()                    
                }).FirstOrDefault(x => x.Id == id);

                if (dentistOfficeEntity == null) return await Task.FromResult<DentistOfficeDto>(null);

                return await Task.FromResult(dentistOfficeEntity);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist office by id {id} with services failed.", ex);
                return await Task.FromResult<DentistOfficeDto>(null);
            }
        }

        public async Task<bool> RemoveAsync(int dentistOfficeId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistOfficeEntity = _dbContext.DentistOffices.FirstOrDefault(x => x.Id == dentistOfficeId);
                    if (dentistOfficeEntity == null) return await Task.FromResult(true);

                    _dbContext.DentistOffices.Remove(dentistOfficeEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove dentist office with id {dentistOfficeId} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> UpdateAsync(DentistOfficeDto dentistOffice)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistOfficeEntity = _dbContext.DentistOffices.FirstOrDefault(x => x.Id == dentistOffice.Id);
                    if (dentistOfficeEntity == null) return await Task.FromResult(false);

                    dentistOfficeEntity.Update(_mapper.Map<DentistOfficeDto, DentistOffice>(dentistOffice));

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update dentist office with id {dentistOffice.Id} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<DentistOfficeDto> GetByIWithRecepcionistAndDentistAsync(int id)
        {
            try
            {
                var dentistOfficeDto = _dbContext.DentistOffices.Select(o => new DentistOfficeDto
                {
                    Id = o.Id,
                    Name = o.Name,
                    Dentists = _mapper.Map<IEnumerable<Dentist>, IEnumerable<DentistDto>>(o.DentistDentistOffices.Select(dentOff => dentOff.Dentist).ToList()),
                    Receptionists = _mapper.Map<IEnumerable<Receptionist>, IEnumerable<ReceptionistDto>>(o.Receptionists)
                }).FirstOrDefault(x => x.Id == id);

                return await Task.FromResult(dentistOfficeDto);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist office by id {id} failed.", ex);
                return await Task.FromResult<DentistOfficeDto>(null);
            }
        }

        public async Task<DentistOfficeDto> GetByIWithDentistsAsync(int id)
        {
            try
            {
                var dentistOfficeDto = _dbContext.DentistOffices.Select(o => new DentistOfficeDto
                {
                    Id = o.Id,
                    Dentists = _mapper.Map<IEnumerable<Dentist>, IEnumerable<DentistDto>>(o.DentistDentistOffices.Select(dentOff => dentOff.Dentist).ToList())
                }).FirstOrDefault(x => x.Id == id);

                return await Task.FromResult(dentistOfficeDto);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist office by id with dentists {id} failed.", ex);
                return await Task.FromResult<DentistOfficeDto>(null);
            }
        }

        public async Task<bool> CreateRecepcionistAsync(ReceptionistDto receptionist)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistOfficeEntity = _dbContext.DentistOffices.Include(x => x.Receptionists).FirstOrDefault(x => x.Id == receptionist.DentistOffice.Id);
                    if (dentistOfficeEntity == null) return await Task.FromResult(false);

                    var receptionistEntity = new Receptionist
                    {
                        ProfileId = receptionist.ProfileId,
                        Email = receptionist.Email,
                        LastName = receptionist.LastName,
                        MobilePhone = receptionist.MobilePhone,
                        Name = receptionist.Name,
                        ProfileImageUrl = receptionist.ProfileImageUrl
                    };

                    dentistOfficeEntity.Receptionists.Add(receptionistEntity);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of recepcionist for email {receptionist.Email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> RemoveRecepcionistAsync(ReceptionistDto receptionist)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistOfficeEntity = _dbContext.DentistOffices.Include(x => x.Receptionists).FirstOrDefault(x => x.Id == receptionist.DentistOffice.Id);
                    if (dentistOfficeEntity == null) return await Task.FromResult(false);

                    var recepcionistToRemove = dentistOfficeEntity.Receptionists.FirstOrDefault(x => x.ProfileId == receptionist.ProfileId);
                    dentistOfficeEntity.Receptionists.Remove(recepcionistToRemove);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove recepcionist with email {receptionist.Email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> AddDentistAsync(int dentistId, int dentistOfficeId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistOfficeEntity = _dbContext.DentistOffices.Include(x => x.DentistDentistOffices).FirstOrDefault(y => y.Id == dentistOfficeId);
                    if (dentistOfficeEntity == null) return await Task.FromResult(false);

                    var dentistEntity = _dbContext.Dentists.FirstOrDefault(x => x.Id == dentistId);
                    if (dentistEntity == null) return await Task.FromResult(false);

                    dentistOfficeEntity.DentistDentistOffices.Add(new DentistDentistOffice
                    {
                        Dentist = dentistEntity,
                        DentistId = dentistEntity.Id,
                        DentistOffice = dentistOfficeEntity,
                        DentistOfficeId = dentistOfficeEntity.Id
                    });

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Adding of dentist with id {dentistId} to dentist office failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> RemoveDentistAsync(int dentistId, int dentistOfficeId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistOfficeEntity = _dbContext.DentistOffices.Include(x => x.DentistDentistOffices).FirstOrDefault(x => x.Id == dentistOfficeId);
                    if (dentistOfficeEntity == null) return await Task.FromResult(false);

                    var dentistDentistOffice = dentistOfficeEntity.DentistDentistOffices.FirstOrDefault(x => x.DentistId == dentistId && x.DentistOfficeId == dentistOfficeId);
                    if (dentistDentistOffice == null) return await Task.FromResult(true);

                    dentistOfficeEntity.DentistDentistOffices.Remove(dentistDentistOffice);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove dentist with id {dentistId} from dentist office with id {dentistOfficeId} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> AddServiceAsync(int serviceId, int dentistOfficeId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistOfficeEntity = _dbContext.DentistOffices.Include(x => x.Services).FirstOrDefault(y => y.Id == dentistOfficeId);
                    if (dentistOfficeEntity == null) return await Task.FromResult(false);

                    var serviceEntity = _dbContext.Services.FirstOrDefault(x => x.Id == serviceId);
                    if (serviceEntity == null) return await Task.FromResult(false);

                    dentistOfficeEntity.Services.Add(serviceEntity);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Adding of service with id {serviceId} to dentist office failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> RemoveServiceAsync(int serviceId, int dentistOfficeId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistOfficeEntity = _dbContext.DentistOffices.Include(x => x.Services).FirstOrDefault(x => x.Id == dentistOfficeId);
                    if (dentistOfficeEntity == null) return await Task.FromResult(false);

                    var serviceEntity = dentistOfficeEntity.Services.FirstOrDefault(x => x.Id == serviceId);
                    if (serviceEntity == null) return await Task.FromResult(true);

                    dentistOfficeEntity.Services.Remove(serviceEntity);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove service with id {serviceId} from dentist office with id {dentistOfficeId} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<IEnumerable<string>> GetDentistOficesCitiesAsync()
        {
            try
            {
                var cities = _dbContext.DentistOffices.Select(o => o.City).Distinct().ToList();

                return await Task.FromResult(cities);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist office cities failed.", ex);
                return await Task.FromResult(Enumerable.Empty<string>());
            }
        }

        public async Task<IEnumerable<DentistOfficeDto>> GetDentistOficesForCityAsync(string cityName)
        {
            try
            {
                var dentistOfficeDto = _dbContext.DentistOffices
                    .Where(x => x.City == cityName)
                    .Select(o => new DentistOfficeDto
                    {
                        Id = o.Id,
                        Name = o.Name
                    }).ToList();

                return await Task.FromResult(dentistOfficeDto);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting dentist offices for city failed.", ex);
                return await Task.FromResult(Enumerable.Empty<DentistOfficeDto>());
            }
        }
    }
}