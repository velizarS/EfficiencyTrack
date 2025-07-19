using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Implementations
{
    public class RoutingService : CrudService<Routing>, IRoutingService
    {

        public RoutingService(EfficiencyTrackDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
        }

        public override async Task<Routing> AddAsync(Routing entity)
        {
            await EnsureRoutingIsUniqueAsync(entity);
            await base.AddAsync(entity);
            return entity;
        }

        public override async Task<bool> UpdateAsync(Routing entity)
        {
            await EnsureRoutingIsUniqueForUpdateAsync(entity);
            await base.UpdateAsync(entity);
            return true;
        }

        public async Task<Routing?> GetByIdWithDepartmentAsync(Guid id)
        {
            return await _context.Routings
                .Include(r => r.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
        }

        public async Task<IEnumerable<Routing>> GetAllWithDepartmentsAsync()
        {
            return await _context.Routings
                .Include(r => r.Department)
                .Where(r => !r.IsDeleted)
                .AsNoTracking()
                .ToListAsync();
        }

        public IQueryable<Routing> GetFilteredRoutings(string? searchTerm, string? sortBy, bool sortAsc)
        {
            IQueryable<Routing> query = _context.Routings
                .Include(r => r.Department)
                .Where(r => !r.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerTerm = searchTerm.ToLower();
                query = query.Where(r =>
                    r.Code.ToLower().Contains(lowerTerm) ||
                    r.Department.Name.ToLower().Contains(lowerTerm));
            }

            query = (sortBy?.ToLower()) switch
            {
                "code" => sortAsc ? query.OrderBy(r => r.Code) : query.OrderByDescending(r => r.Code),
                "department" => sortAsc ? query.OrderBy(r => r.Department.Name) : query.OrderByDescending(r => r.Department.Name),
                "minutes" => sortAsc ? query.OrderBy(r => r.MinutesPerPiece) : query.OrderByDescending(r => r.MinutesPerPiece),
                "zone" => sortAsc ? query.OrderBy(r => r.Zone) : query.OrderByDescending(r => r.Zone),
                _ => query.OrderBy(r => r.Code)
            };

            return query;
        }

        public async Task<Routing> GetRoutingByCodeAsync(string routingCode)
        {
            return await _context.Routings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == routingCode && !r.IsDeleted);
        }

        private async Task EnsureRoutingIsUniqueAsync(Routing entity)
        {
            bool exists = await _context.Routings.AsNoTracking().AnyAsync(r => r.Code == entity.Code && !r.IsDeleted);
            if (exists)
            {
                throw new InvalidOperationException($"Routing with code {entity.Code} already exists.");
            }
        }

        private async Task EnsureRoutingIsUniqueForUpdateAsync(Routing entity)
        {
            bool exists = await _context.Routings.AsNoTracking()
                .AnyAsync(r => r.Code == entity.Code && r.Id != entity.Id && !r.IsDeleted);

            if (exists)
            {
                throw new InvalidOperationException($"Another Routing with code {entity.Code} already exists.");
            }
        }
    }
}
