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
            var users = await _userManager.GetUsersInRoleAsync("ShiftManagerUser");
            return users.ToList();
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


    }
}
