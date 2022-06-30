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
    public class AdminRepository : IAdminRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly DentistCalendarDbContext _dbContext;

        public AdminRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(AdminDto admin)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var adminEntity = _dbContext.Admins.FirstOrDefault(x => x.ProfileId == admin.ProfileId);
                    if (adminEntity != null) return await Task.FromResult(false);

                    adminEntity = new Admin
                    {
                        ProfileId = admin.ProfileId,
                        Name = admin.Name,
                        NIP = admin.NIP,
                        LastName = admin.LastName,
                        Adress = admin.Adress,
                        MobilePhone = admin.MobilePhone,
                        ProfileImageUrl = admin.ProfileImageUrl,
                        LicenseType = admin.LicenseType,
                        LicenseStartDate = admin.LicenseStartDate,
                        LicenseEndDate = admin.LicenseEndDate
                    };

                    await _dbContext.Admins.AddAsync(adminEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of admin with profileId: {admin.ProfileId} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> CreateDentistOfficeAsync(DentistOfficeDto dentistOffice)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var adminEntity = _dbContext.Admins.Include(x => x.DentistOffices).FirstOrDefault(x => x.ProfileId == dentistOffice.Admin.ProfileId);
                    if (adminEntity == null) return await Task.FromResult(false);

                    var dentistOfficeEntity = new DentistOffice
                    {
                        AboutUs = dentistOffice.AboutUs,
                        Adress = dentistOffice.Adress,
                        City = dentistOffice.City,
                        Email = dentistOffice.Email,
                        Name = dentistOffice.Name,
                        Phone = dentistOffice.Phone
                    };

                    adminEntity.DentistOffices.Add(dentistOfficeEntity);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of dentist office for email {dentistOffice.Email} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<AdminDto> GetByIdAsync(int id)
        {
            try
            {
                var adminEntity = _dbContext.Admins.FirstOrDefault(x => x.Id == id);
                if (adminEntity == null) return await Task.FromResult<AdminDto>(null);

                return await Task.FromResult(_mapper.Map<Admin, AdminDto>(adminEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Get admin with id: {id} failed.", ex);
                return await Task.FromResult<AdminDto>(null);
            }
        }

        public async Task<AdminDto> GetByProfileIdAsync(Guid id)
        {
            try
            {
                var adminEntity = _dbContext.Admins.FirstOrDefault(x => x.ProfileId == id);
                if (adminEntity == null) return await Task.FromResult<AdminDto>(null);

                return await Task.FromResult(_mapper.Map<Admin, AdminDto>(adminEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Get admin with profile id: {id.ToString()} failed.", ex);
                return await Task.FromResult<AdminDto>(null);
            }
        }

        public async Task<AdminDto> GetByProfileIdWithDentistOfficesAsync(Guid id)
        {
            try
            {
                var adminEntity = _dbContext.Admins.Include(x => x.DentistOffices).FirstOrDefault(x => x.ProfileId == id);
                if (adminEntity == null) return await Task.FromResult<AdminDto>(null);

                return await Task.FromResult(_mapper.Map<Admin, AdminDto>(adminEntity));
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Get admin with profile id: {id.ToString()} failed.", ex);
                return await Task.FromResult<AdminDto>(null);
            }
        }

        public async Task<bool> RemoveAsync(AdminDto admin)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var adminEntity = _dbContext.Admins.FirstOrDefault(x => x.ProfileId == admin.ProfileId);
                    if (adminEntity == null) return await Task.FromResult(true);

                    _dbContext.Admins.Remove(adminEntity);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Removing admin with email: {admin.ProfileId} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> UpdateAsync(AdminDto admin)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var adminEntity = _dbContext.Admins.FirstOrDefault(x => x.ProfileId == admin.ProfileId);
                    if (adminEntity == null) return await Task.FromResult(false);

                    adminEntity.Update(_mapper.Map<AdminDto, Admin>(admin));

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Update of admin with email: {admin.ProfileId} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }
    }
}