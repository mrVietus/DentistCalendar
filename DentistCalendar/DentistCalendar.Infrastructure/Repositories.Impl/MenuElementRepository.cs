using AutoMapper;
using AutoMapper.QueryableExtensions;
using DentistCalendar.Common.Enums;
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
    public class MenuElementRepository : IMenuElementRepository
    {
        private readonly ILoggerService _loggerService;
        private readonly IMapper _mapper;
        private readonly DentistCalendarDbContext _dbContext;

        public MenuElementRepository(DentistCalendarDbContext dentistCalendarDbContext, ILoggerService loggerService, IMapper mapper)
        {
            _dbContext = dentistCalendarDbContext;
            _loggerService = loggerService;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(MenuElement menuElement)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var menuElementEntity = _dbContext.MenuElements.FirstOrDefault(x => x.Id == menuElement.Id);
                    if (menuElementEntity != null) return await Task.FromResult(false);

                    await _dbContext.MenuElements.AddAsync(menuElement);
                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Creation of menu element with name: {menuElement.DisplayName} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<ICollection<MenuElement>> GetMenuElementsAsync()
        {
            try
            {
                var menuElements = _dbContext.MenuElements.ToList();
                if (menuElements == null) return await Task.FromResult<ICollection<MenuElement>>(null);

                return await Task.FromResult(menuElements);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Get menu elements failed.", ex);
                return await Task.FromResult<ICollection<MenuElement>>(null);
            }
        }

        public async Task<IEnumerable<MenuElementDto>> GetMenuElementsForAccountTypeAsync(AccountType accountType)
        {
            try
            {
                var menuElements = await _dbContext.MenuElements.Where(me => me.TypeOfPermittedAccount == accountType)
                    .ProjectTo<MenuElementDto>(_mapper.ConfigurationProvider).ToListAsync();
                if (menuElements == null) return await Task.FromResult<IEnumerable<MenuElementDto>>(null);

                return await Task.FromResult(menuElements);
            }
            catch (Exception ex)
            {
                _loggerService.Error($"Get menu elements failed.", ex);
                return await Task.FromResult<IEnumerable<MenuElementDto>>(null);
            }
        }

        public async Task<bool> UpdateAsync(MenuElement menuElement, int menuElementId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var menuElementEntity = _dbContext.MenuElements.FirstOrDefault(x => x.Id == menuElementId);
                    if (menuElementEntity == null) return await Task.FromResult(false);

                    menuElementEntity.Update(menuElement);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Updating of menu element with name: {menuElement.DisplayName} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }

        public async Task<bool> RemoveAsync(int menuElementId)
        {
            using (var dbContextTransaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var menuElementEntity = _dbContext.MenuElements.FirstOrDefault(x => x.Id == menuElementId);
                    if (menuElementEntity == null) return await Task.FromResult(true);

                    _dbContext.MenuElements.Remove(menuElementEntity);

                    _dbContext.SaveChanges();
                    dbContextTransaction.Commit();

                    return await Task.FromResult(true);
                }
                catch (Exception ex)
                {
                    _loggerService.Error($"Remove of menu element with id: {menuElementId} failed.", ex);
                    dbContextTransaction.Rollback();
                    return await Task.FromResult(false);
                }
            }
        }
    }
}