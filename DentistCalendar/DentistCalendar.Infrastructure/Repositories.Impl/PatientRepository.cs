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
    public class PatientRepository : IPatientRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly DentistCalendarDbContext _dbContext;

        public PatientRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(PatientDto patient)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var patientEntity = _dbContext.Patients.FirstOrDefault(x => x.Id == patient.Id);
                    if (patientEntity != null) return await Task.FromResult(false);

                    patientEntity = new Patient
                    {
                        ProfileId = patient.ProfileId,
                        Name = patient.Name,
                        LastName = patient.LastName,
                        MobilePhone = patient.MobilePhone,
                        ProfileImageUrl = patient.ProfileImageUrl
                    };

                    await _dbContext.Patients.AddAsync(patientEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of patient for email {patient.Email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<PatientDto> GetByIdAsync(int id)
        {
            try
            {
                var patientEntity = _dbContext.Patients.FirstOrDefault(x => x.Id == id);
                if (patientEntity == null) return await Task.FromResult<PatientDto>(null);

                return await Task.FromResult(_mapper.Map<Patient, PatientDto>(patientEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting patient by id {id} failed.", ex);
                return await Task.FromResult<PatientDto>(null);
            }
        }

        public async Task<PatientDto> GetByProfileIdAsync(Guid id)
        {
            try
            {
                var patientEntity = _dbContext.Patients.FirstOrDefault(x => x.ProfileId == id);
                if (patientEntity == null) return await Task.FromResult<PatientDto>(null);

                return await Task.FromResult(_mapper.Map<Patient, PatientDto>(patientEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Getting patient by profile id {id} failed.", ex);
                return await Task.FromResult<PatientDto>(null);
            }
        }

        public async Task<bool> RemoveAsync(PatientDto patient)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var patientEntity = _dbContext.Patients.FirstOrDefault(x => x.Id == patient.Id);
                    if (patientEntity == null) return await Task.FromResult(true);

                    _dbContext.Patients.Remove(patientEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove patient with id {patient.Id} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> UpdateAsync(PatientDto patient)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var patientEntity = _dbContext.Patients.FirstOrDefault(x => x.ProfileId == patient.ProfileId);
                    if (patientEntity == null) return await Task.FromResult(false);

                    patientEntity.Update(_mapper.Map<PatientDto, Patient>(patient));

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update patient with id {patient.Id} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }
    }
}