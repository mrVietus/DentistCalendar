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
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly DentistCalendarDbContext _dbContext;

        public AppointmentRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(int patientId, int dentistId, int serviceId, DateTime appointmentDate)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var dentistEntity = _dbContext.Dentists.Where(x => x.Id == dentistId).FirstOrDefault();
                    if (dentistEntity == null) return await Task.FromResult(false);

                    var serviceEntity = _dbContext.Services
                        .Include(x => x.DentistServices)
                        .Include(y => y.DentistOffice)
                        .Where(z => z.Id == serviceId).FirstOrDefault();
                    if (serviceEntity == null) return await Task.FromResult(false);
                    if (serviceEntity.DentistServices.FirstOrDefault(x => x.DentistId == dentistEntity.Id) == null) return await Task.FromResult(false);

                    var appointmentEntity = _dbContext.Appointments
                        .FirstOrDefault(x => x.DentistOffice == serviceEntity.DentistOffice && x.Dentist.Id == dentistEntity.Id && x.StartDate == appointmentDate);
                    if (appointmentEntity != null) return await Task.FromResult(false);

                    var patientEntity = _dbContext.Patients.FirstOrDefault(x => x.Id == patientId);
                    if (patientEntity == null) return await Task.FromResult(false);

                    var newAppointment = new Appointment
                    {
                        Dentist = dentistEntity,
                        DentistOffice = serviceEntity.DentistOffice,
                        Service = serviceEntity,
                        Patient = patientEntity,
                        StartDate = appointmentDate
                    };

                    await _dbContext.Appointments.AddAsync(newAppointment);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of appointment for patient: {patientId} with dentist: {dentistId} for service: {serviceId} on date {appointmentDate} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllForDentistAsync(DentistDto dentist)
        {
            try
            {
                var appointments = await _dbContext.Appointments
                    .Include(x => x.Service)
                    .Include(x => x.Patient)
                    .Include(x => x.Dentist)
                    .Include(x => x.DentistOffice)
                    .Where(x => x.Dentist.Id == dentist.Id)
                    .ProjectTo<AppointmentDto>(_mapper.ConfigurationProvider).ToListAsync();

                if (appointments == null || !appointments.Any()) return await Task.FromResult<IEnumerable<AppointmentDto>>(null);

                return await Task.FromResult(appointments);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting all appointments for dentist: {dentist.Email} failed.", ex);
                return await Task.FromResult<IEnumerable<AppointmentDto>>(null);
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllForDentistOfficeAsync(DentistOfficeDto dentistOffice)
        {
            try
            {
                var appointments = await _dbContext.Appointments
                    .Include(x => x.Service)
                    .Include(x => x.Patient)
                    .Include(x => x.Dentist)
                    .Include(x => x.DentistOffice)
                    .Where(x => x.DentistOffice.Id == dentistOffice.Id)
                    .ProjectTo<AppointmentDto>(_mapper.ConfigurationProvider).ToListAsync();

                if (appointments == null || !appointments.Any()) return await Task.FromResult<IEnumerable<AppointmentDto>>(null);

                return await Task.FromResult(appointments);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting all appointments for dentist office: {dentistOffice.Email} failed.", ex);
                return await Task.FromResult<IEnumerable<AppointmentDto>>(null);
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllForPatientAsync(PatientDto patient)
        {
            try
            {
                var appointments = _dbContext.Appointments
                    .Include(x => x.Service)
                    .Include(x => x.Dentist)
                    .Include(x => x.DentistOffice).Where(x => x.Patient.Id == patient.Id)
                    .Select(o => new AppointmentDto
                    {
                        Dentist = _mapper.Map<Dentist, DentistDto>(o.Dentist),
                        DentistOffice = _mapper.Map<DentistOffice, DentistOfficeDto>(o.DentistOffice),
                        Id = o.Id,
                        Service = _mapper.Map<Service, ServiceDto>(o.Service),
                        StartDate = o.StartDate
                    })
                    .ToList();

                if (appointments == null || !appointments.Any()) return Enumerable.Empty<AppointmentDto>();

                return await Task.FromResult(appointments);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting all appointments for patient: {patient.Email} failed.", ex);
                return Enumerable.Empty<AppointmentDto>();
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetForDentistOfficeWithDateAsync(DentistOfficeDto dentistOffice, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var appointments = await _dbContext.Appointments
                    .Include(x => x.Service)
                    .Include(x => x.Patient)
                    .Include(x => x.Dentist)
                    .Include(x => x.DentistOffice)
                    .Where(x => x.DentistOffice.Id == dentistOffice.Id && x.StartDate >= fromDate && x.StartDate <= toDate)
                    .ProjectTo<AppointmentDto>(_mapper.ConfigurationProvider).ToListAsync();
                if (appointments == null || !appointments.Any()) return await Task.FromResult<IEnumerable<AppointmentDto>>(null);

                return await Task.FromResult(appointments);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting all appointments for dentist office: {dentistOffice.Email} with date from {fromDate} to {toDate} failed.", ex);
                return await Task.FromResult<IEnumerable<AppointmentDto>>(null);
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetForDentistWithDateAsync(int dentistId, DateTime date)
        {
            try
            {
                var midnight = SetMidnightTime(date);

                var appointments = await _dbContext.Appointments
                    .Include(x => x.Dentist)
                    .Where(x => x.Dentist.Id == dentistId && x.StartDate >= date && x.StartDate < midnight)
                    .ProjectTo<AppointmentDto>(_mapper.ConfigurationProvider).ToListAsync();

                if (appointments == null || !appointments.Any()) return Enumerable.Empty<AppointmentDto>();

                return await Task.FromResult(appointments);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting all appointments for dentist: {dentistId} with date {date}, failed.", ex);
                return Enumerable.Empty<AppointmentDto>();
            }
        }

        private DateTime SetMidnightTime(DateTime date)
        {
            date.AddHours(23 - date.Hour);
            date.AddMinutes(59 - date.Minute);
            return date;
        }

        public async Task<IEnumerable<AppointmentDto>> GetForPatientWithDate(PatientDto patient, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var appointments = await _dbContext.Appointments
                    .Include(x => x.Service)
                    .Include(x => x.Patient)
                    .Include(x => x.Dentist)
                    .Include(x => x.DentistOffice)
                    .Where(x => x.DentistOffice.Id == patient.Id && x.StartDate >= fromDate && x.StartDate <= toDate)
                    .ProjectTo<AppointmentDto>(_mapper.ConfigurationProvider).ToListAsync();

                if (appointments == null || !appointments.Any()) return await Task.FromResult<IEnumerable<AppointmentDto>>(null);

                return await Task.FromResult(appointments);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting all appointments for patient: {patient.Email} with date from {fromDate} to {toDate} failed.", ex);
                return await Task.FromResult<IEnumerable<AppointmentDto>>(null);
            }
        }

        public Task<bool> RemoveAsync(int id)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var appointmentEntity = _dbContext.Appointments.FirstOrDefault(x => x.Id == id);
                    if (appointmentEntity == null) return Task.FromResult(false);

                    _dbContext.Appointments.Remove(appointmentEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove appointment with id {id} failed", ex);
                    dbContextTransaction.Rollback();
                    return Task.FromResult(false);
                }
            }
        }

        public Task<bool> UpdateAsync(AppointmentDto appointment)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var appointmentEntity = _dbContext.Appointments
                        .Include(x => x.Service)
                        .Include(x => x.Patient)
                        .Include(x => x.Dentist)
                        .Include(x => x.DentistOffice)
                        .FirstOrDefault(x => x.Id == appointment.Id);

                    if (appointmentEntity == null) return Task.FromResult(false);

                    appointmentEntity.Update(_mapper.Map<AppointmentDto, Appointment>(appointment));
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update appointment with id {appointment.Id} failed", ex);
                    dbContextTransaction.Rollback();
                    return Task.FromResult(false);
                }
            }
        }
    }
}