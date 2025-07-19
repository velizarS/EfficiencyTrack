using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Helpers;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Implementations
{
    public class DepartmentService : CrudService<Department>, IDepartmentService
    {
        public DepartmentService(
            EfficiencyTrackDbContext context,
            IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        public override async Task<Department> AddAsync(Department entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await ValidateDepartmentUniquenessForCreateAsync(entity);
            return await base.AddAsync(entity);
        }

        public override async Task<bool> UpdateAsync(Department entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await ValidateDepartmentUniquenessForUpdateAsync(entity);
            return await base.UpdateAsync(entity);
        }

        public async Task<Department?> GetDepartmentWithEmployeesAsync(Guid id)
        {
            return await _context.Departments
                .AsNoTracking()
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

        private async Task ValidateDepartmentUniquenessForCreateAsync(Department entity)
        {
            bool exists = await _context.Departments.AsNoTracking()
                .AnyAsync(e => e.Name == entity.Name && !e.IsDeleted);

            if (exists)
                throw new DuplicateDepartmentException($"Department with name '{entity.Name}' already exists.");
        }

        private async Task ValidateDepartmentUniquenessForUpdateAsync(Department entity)
        {
            bool exists = await _context.Departments.AsNoTracking()
                .AnyAsync(e => e.Name == entity.Name && e.Id != entity.Id && !e.IsDeleted);

            if (exists)
                throw new DuplicateDepartmentException($"Another Department with name '{entity.Name}' already exists.");
        }
    }
}
