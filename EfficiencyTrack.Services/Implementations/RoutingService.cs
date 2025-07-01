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
    }
}
