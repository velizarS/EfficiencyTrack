using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Identity;
using Microsoft.AspNetCore.Identity;

namespace EfficiencyTrack.Services
{
    public class EmployeeService : CrudService<Employee>, IEmployeeService
    {
        private readonly EfficiencyTrackDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;


        public EmployeeService(EfficiencyTrackDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
      : base(context, httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;

        }

        public async Task<List<ApplicationUser>> GetAllShiftManagersAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("ShiftLeader");
            return users.ToList();
        }

        public override async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.ShiftManagerUser)
                .Where(e => !e.IsDeleted)
                .ToListAsync(); 
        }

        public override async Task<Employee> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.ShiftManagerUser)
                .Where(e => !e.IsDeleted && e.Id == id)
                .FirstOrDefaultAsync();
        }

        public override async Task AddAsync(Employee entity)
        {
            await EnsureEmployIsUniqueAsync(entity);
            await base.AddAsync(entity);
        }

        public override async Task UpdateAsync(Employee entity)
        {
            await EnsureEmployIsUniqueForUpdateAsync(entity);
            await base.UpdateAsync(entity);
        }

        public async Task<Employee> GetByCodeAsync(string employeeCode)
        {
            return await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Code == employeeCode && !e.IsDeleted);
        }

        public async Task<List<Employee>> GetByDepartmentAsync(Guid departmentId)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.DepartmentId == departmentId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetByShiftManagerUserIdAsync(Guid leaderId)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.ShiftManagerUserId == leaderId && !e.IsDeleted)
                .ToListAsync();
        }

        public async Task<bool> IsEmployeeCodeUniqueAsync(string code, Guid? excludeId = null)
        {
            return !await _context.Employees
                .AsNoTracking()
                .AnyAsync(e => e.Code == code && (!excludeId.HasValue || e.Id != excludeId.Value));
        }

        private async Task EnsureEmployIsUniqueForUpdateAsync(Employee entity)
        {
            var exists = await _context.Employees.AsNoTracking()
                .AnyAsync(e => e.Code == entity.Code && e.Id != entity.Id && !e.IsDeleted);

            if (exists)
                throw new InvalidOperationException($"Another Employ with code {entity.Code} already exists.");
        }

        private async Task EnsureEmployIsUniqueAsync(Employee entity)
        {
            var exists = await _context.Employees.AsNoTracking().AnyAsync(e => e.Code == entity.Code && !e.IsDeleted);
            if (exists)
                throw new InvalidOperationException($"Routing with code {entity.Code} already exists.");
        }
    }
}
