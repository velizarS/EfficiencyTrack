using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Helpers;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Implementations
{
    public class DepartmentService : CrudService<Department>, IDepartmentService
    {
        private readonly EfficiencyTrackDbContext _context;

        public DepartmentService(
            EfficiencyTrackDbContext context,
            IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            _context = context;
        }

        public override async Task AddAsync(Department entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await EnsureDepartmentIsUniqueAsync(entity);
            await base.AddAsync(entity);
        }

        public override async Task UpdateAsync(Department entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await EnsureDepartmentIsUniqueForUpdateAsync(entity);
            await base.UpdateAsync(entity);
        }

        public async Task<Department?> GetDepartmentWithEmployeesAsync(Guid id)
        {
            return await _context.Departments
                .AsNoTracking()
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }


        private async Task EnsureDepartmentIsUniqueForUpdateAsync(Department entity)
        {
            var exists = await _context.Departments.AsNoTracking()
                .AnyAsync(e => e.Name == entity.Name && e.Id != entity.Id && !e.IsDeleted);

            if (exists)
                throw new DuplicateDepartmentException($"Another Department with name '{entity.Name}' already exists.");
        }

        private async Task EnsureDepartmentIsUniqueAsync(Department entity)
        {
            var exists = await _context.Departments.AsNoTracking()
                .AnyAsync(e => e.Name == entity.Name && !e.IsDeleted);

            if (exists)
                throw new DuplicateDepartmentException($"Department with name '{entity.Name}' already exists.");
        }
    }
}
