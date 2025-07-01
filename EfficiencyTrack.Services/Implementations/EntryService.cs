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
    public class EntryService : CrudService<Entry>, IEntryService
    {
        private readonly IDailyEfficiencyService _dailyEfficiencyService;
        private readonly EfficiencyTrackDbContext _context;

        public EntryService(EfficiencyTrackDbContext context, IHttpContextAccessor httpContextAccessor, IDailyEfficiencyService dailyEfficiencyService)
            : base(context, httpContextAccessor)
        {
            _dailyEfficiencyService = dailyEfficiencyService;
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Entry>> GetAllWithIncludesAsync()
        {
            return await _context.Entries
                 .Include(e => e.Employee)
                 .Include(e => e.Routing)
                 .Where(e => !e.IsDeleted)
                 .ToListAsync();
        }

        public async Task<Entry?> GetByIdWithIncludesAsync(Guid id)
        {
            return await _context.Entries
                .Include(e => e.Employee)
                .Include(e => e.Routing)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public override async Task AddAsync(Entry entity)
        {
            await SetEfficiencyAsync(entity);
            await base.AddAsync(entity);
            await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);
        }

        public override async Task UpdateAsync(Entry entity)
        {
            await SetEfficiencyAsync(entity);
            await base.UpdateAsync(entity);
            await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);
        }

        public override async Task DeleteAsync(Guid id)
        {
            var entity = await base.GetByIdAsync(id);
            if (entity != null)
            {
                await base.DeleteAsync(id);
                await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);
            }
        }

        private decimal CalculateEfficiency(decimal requiredMinutes, decimal workedMinutes)
        {
            if (workedMinutes <= 0)
                throw new InvalidOperationException("Worked minutes must be greater than zero.");

            return (requiredMinutes / workedMinutes) * 100;
        }

        private async Task SetEfficiencyAsync(Entry entry)
        {
            var routing = await _context.Routings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == entry.RoutingId);

            if (routing == null)
                throw new InvalidOperationException("Invalid RoutingId");

            var requiredMinutes = (entry.Pieces + entry.Scrap) * routing.MinutesPerPiece;

            entry.EfficiencyForOperation = CalculateEfficiency(requiredMinutes, entry.WorkedMinutes);
        }

        async Task IEntryService.SetEfficiencyAsync(Entry entry)
        {
            await SetEfficiencyAsync(entry);
        }
    }
}
