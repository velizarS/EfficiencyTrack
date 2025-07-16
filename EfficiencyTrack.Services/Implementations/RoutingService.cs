using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Implementations
{
    public class RoutingService : CrudService<Routing>, IRoutingService
    {
        private readonly EfficiencyTrackDbContext _context;

        public RoutingService(EfficiencyTrackDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            _context = context;
        }

        public override async Task AddAsync(Routing entity)
        {
            await EnsureRoutingIsUniqueAsync(entity);
            await base.AddAsync(entity);
        }

        public override async Task UpdateAsync(Routing entity)
        {
            await EnsureRoutingIsUniqueForUpdateAsync(entity);
            await base.UpdateAsync(entity);
        }

        public async Task<Routing?> GetByIdWithDepartmentAsync(Guid id)
        {
            return await _context.Routings
                .Include(r => r.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<Routing> GetRoutingByCodeAsync(string routingCode)
        {
            return await _context.Routings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == routingCode && !r.IsDeleted);
        }

        public async Task<IEnumerable<Routing>> GetAllWithDepartmentsAsync()
        {
            return await _context.Routings
                .Include(r => r.Department)
                .Where(r => !r.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        private async Task EnsureRoutingIsUniqueAsync(Routing entity)
        {
            var exists = await _context.Routings.AsNoTracking().AnyAsync(r => r.Code == entity.Code && !r.IsDeleted);
            if (exists)
                throw new InvalidOperationException($"Routing with code {entity.Code} already exists.");
        }

        private async Task EnsureRoutingIsUniqueForUpdateAsync(Routing entity)
        {
            var exists = await _context.Routings.AsNoTracking()
                .AnyAsync(r => r.Code == entity.Code && r.Id != entity.Id && !r.IsDeleted);

            if (exists)
                throw new InvalidOperationException($"Another Routing with code {entity.Code} already exists.");
        }
    }
}
