using AutoMapper;
using DentistCalendar.Common.Logger;
using DentistCalendar.Core.Entities;
using DentistCalendar.Dto.DTO.Application;
using DentistCalendar.Dto.DTO.Domain;
using DentistCalendar.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DentistCalendar.Infrastructure.Repositories.Impl
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly DentistCalendarDbContext _dbContext;

        public ServiceRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<ServiceAddResponse> CreateAsync(ServiceDto service)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var serviceEntity = new Service
                    {
                        Description = service.Description,
                        Name = service.Name,
                        Price = service.Price,
                        Time = service.Time
                    };

                    var addedService = await _dbContext.Services.AddAsync(serviceEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(new ServiceAddResponse { WasServiceAdded = true, Serviceid = addedService.Entity.Id });
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of service for dentist office {service.DentistOffice} with {service.Name} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(new ServiceAddResponse { WasServiceAdded = false });
                }
            }
        }

        public async Task<bool> AddAssignedDentistsAsync(IEnumerable<int> dentistsIds, int serviceId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var serviceEntity = _dbContext.Services.Include(x => x.DentistServices).FirstOrDefault(y => y.Id == serviceId);
                    if (serviceEntity == null) return await Task.FromResult(false);

                    foreach (var dentistId in dentistsIds)
                    {
                        var dentistEntity = _dbContext.Dentists.FirstOrDefault(x => x.Id == dentistId);
                        if (dentistEntity == null) return await Task.FromResult(false);

                        serviceEntity.DentistServices.Add(new DentistService
                        {
                            Dentist = dentistEntity,
                            DentistId = dentistEntity.Id,
                            Service = serviceEntity,
                            ServiceId = serviceEntity.Id
                        });
                    }

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Connection dentists with service with service Id {serviceId} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> EditAssignedDentistsAsync(IEnumerable<int> dentistsIds, int serviceId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var serviceEntity = _dbContext.Services.Include(x => x.DentistServices).FirstOrDefault(y => y.Id == serviceId);
                    if (serviceEntity == null) return await Task.FromResult(false);

                    if (dentistsIds == null)
                    {
                        serviceEntity.DentistServices.Clear();
                    }
                    else
                    {
                        var dentistServiceToRemove = serviceEntity.DentistServices.Where(x => !dentistsIds.ToList().Contains(x.DentistId.Value)).ToList();
                        if (dentistServiceToRemove.Any())
                        {
                            foreach (var dentistService in dentistServiceToRemove)
                            {
                                serviceEntity.DentistServices.Remove(dentistService);
                            }
                        }

                        foreach (var dentistId in dentistsIds)
                        {
                            var existingConnection = serviceEntity.DentistServices.Where(x => x.DentistId == dentistId).ToList();
                            if (!existingConnection.Any())
                            {
                                var dentistEntity = _dbContext.Dentists.FirstOrDefault(x => x.Id == dentistId);
                                if (dentistEntity == null) return await Task.FromResult(false);

                                serviceEntity.DentistServices.Add(new DentistService
                                {
                                    Dentist = dentistEntity,
                                    DentistId = dentistEntity.Id,
                                    Service = serviceEntity,
                                    ServiceId = serviceEntity.Id
                                });
                            }
                        }
                    }

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update connection dentists with service with service Id {serviceId} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<ServiceDto> GetByIdAsync(int id)
        {
            try
            {
                var serviceEntity = _dbContext.Services.Select(ser => new ServiceDto
                {
                    Id = ser.Id,
                    Dentists = ser.DentistServices.Select(x => new DentistDto { Name = x.Dentist.Name, LastName = x.Dentist.LastName, Id = x.Dentist.Id, DoctorTitle = x.Dentist.DoctorTitle }).ToList(),
                    Description = ser.Description,
                    Name = ser.Name,
                    Price = ser.Price,
                    Time = ser.Time
                }).FirstOrDefault(x => x.Id == id);

                //var serviceEntity = _dbContext.Services.Include(x => x.DentistServices).FirstOrDefault(x => x.Id == id);
                if (serviceEntity == null) return await Task.FromResult<ServiceDto>(null);

                return await Task.FromResult(serviceEntity);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting service by id {id} failed.", ex);
                return await Task.FromResult<ServiceDto>(null);
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var serviceEntity = _dbContext.Services.FirstOrDefault(x => x.Id == id);
                    if (serviceEntity == null) return await Task.FromResult(true);

                    _dbContext.Services.Remove(serviceEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove service with id {id} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> UpdateAsync(ServiceDto service)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var serviceEntity = _dbContext.Services.FirstOrDefault(x => x.Id == service.Id);
                    if (serviceEntity == null) return await Task.FromResult(false);

                    serviceEntity.Update(_mapper.Map<ServiceDto, Service>(service));

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update service with id {service.Id} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }
    }
}