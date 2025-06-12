using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace EfficiencyTrack.Services
{
    public class EmployeeService : CrudService<Employee>, IEmployeeService
    {
        private readonly EfficiencyTrackDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(EfficiencyTrackDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<Employee>> GetAllActiveAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.IsActive)
                .Include(e => e.Department)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetByDepartmentAsync(Guid departmentId)
        {
            return await _context.Employees.AsNoTracking()
                .Where(e => e.DepartmentId == departmentId && e.IsActive)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetByShiftManagerUserIdAsync(Guid leaderId)
        {
            return await _context.Employees.AsNoTracking()
                .Where(e => e.ShiftManagerUserId == leaderId && e.IsActive)
                .ToListAsync();
        }

        public async Task<bool> IsEmployeeCodeUniqueAsync(string code, Guid? excludeId = null)
        {
            return !await _context.Employees.AsNoTracking()
                .AnyAsync(e => e.Code == code && (!excludeId.HasValue || e.Id != excludeId.Value));
        }

        public async Task DeactivateAsync(Guid employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return;

            employee.IsActive = false;

            await _context.SaveChangesAsync();
        }
    }
}
