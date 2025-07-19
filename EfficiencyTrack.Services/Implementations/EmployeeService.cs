using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Implementations
{
    public class EmployeeService : CrudService<Employee>, IEmployeeService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeService(
            EfficiencyTrackDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<ApplicationUser> userManager)
            : base(context, httpContextAccessor)
        {
            _userManager = userManager;
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

        public override async Task<Employee?> GetByIdAsync(Guid id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.ShiftManagerUser)
                .Where(e => !e.IsDeleted && e.Id == id)
                .FirstOrDefaultAsync();
        }

        public override async Task<Employee> AddAsync(Employee entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await EnsureEmployeeIsUniqueAsync(entity);
            await base.AddAsync(entity);
            return entity;
        }

        public override async Task<bool> UpdateAsync(Employee entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await EnsureEmployeeIsUniqueForUpdateAsync(entity);
            await base.UpdateAsync(entity);
            return true;
        }

        public async Task<Employee?> GetByCodeAsync(string employeeCode)
        {
            if (string.IsNullOrWhiteSpace(employeeCode))
                return null;

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
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be null or empty", nameof(code));

            return !await _context.Employees
                .AsNoTracking()
                .AnyAsync(e => e.Code == code && (!excludeId.HasValue || e.Id != excludeId.Value) && !e.IsDeleted);
        }

        private async Task EnsureEmployeeIsUniqueForUpdateAsync(Employee entity)
        {
            bool exists = await _context.Employees.AsNoTracking()
                .AnyAsync(e => e.Code == entity.Code && e.Id != entity.Id && !e.IsDeleted);

            if (exists)
            {
                throw new InvalidOperationException($"Another employee with code '{entity.Code}' already exists.");
            }
        }

        private async Task EnsureEmployeeIsUniqueAsync(Employee entity)
        {
            bool exists = await _context.Employees.AsNoTracking()
                .AnyAsync(e => e.Code == entity.Code && !e.IsDeleted);

            if (exists)
            {
                throw new InvalidOperationException($"Employee with code '{entity.Code}' already exists.");
            }
        }
    }
}
