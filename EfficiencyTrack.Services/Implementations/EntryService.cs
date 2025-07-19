using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Helpers;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Implementations
{
    public class EntryService : CrudService<Entry>, IEntryService
    {
        private readonly IDailyEfficiencyService _dailyEfficiencyService;
        private readonly EntryValidator _validator;
        private readonly IGreetingService _greetingService;

        public EntryService(
            EfficiencyTrackDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IDailyEfficiencyService dailyEfficiencyService,
            IGreetingService greetingService)
            : base(context, httpContextAccessor)
        {
            _dailyEfficiencyService = dailyEfficiencyService ?? throw new ArgumentNullException(nameof(dailyEfficiencyService));
            _greetingService = greetingService ?? throw new ArgumentNullException(nameof(greetingService));
            _validator = new EntryValidator(_context ?? throw new ArgumentNullException(nameof(context)));
        }

        public async Task<List<Entry>> GetAllWithIncludesAsync()
        {
            return await _context.Entries
                .AsNoTracking()
                .Include(e => e.Employee)
                .Include(e => e.Routing)
                .Where(e => !e.IsDeleted)
                .OrderByDescending(e => e.CreatedOn)
                .ToListAsync();
        }

        public async Task<Entry?> GetByIdWithIncludesAsync(Guid id)
        {
            return await _context.Entries
                .AsNoTracking()
                .Include(e => e.Employee)
                .Include(e => e.Routing)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public override async Task<Entry> AddAsync(Entry entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await ValidateAndSetEfficiencyAsync(entity);

            var addedEntity = await base.AddAsync(entity);

            await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);

            return addedEntity;
        }

        public override async Task<bool> UpdateAsync(Entry entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await ValidateAndSetEfficiencyAsync(entity);

            var result = await base.UpdateAsync(entity);

            await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);

            return result;
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await base.GetByIdAsync(id);

            if (entity == null) return false;

            var result = await base.DeleteAsync(id);

            await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);

            return result;
        }

        private async Task ValidateAndSetEfficiencyAsync(Entry entry)
        {
            var validationResult = await _validator.ValidateAsync(entry);
            if (!validationResult.IsValid)
                throw new InvalidOperationException(string.Join(Environment.NewLine, validationResult.Errors));

            await SetEfficiencyAsync(entry);
        }

        private async Task SetEfficiencyAsync(Entry entry)
        {
            var routing = await _context.Routings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == entry.RoutingId);

            if (routing == null)
                throw new InvalidOperationException("Невалиден RoutingId");

            decimal requiredMinutes = (entry.Pieces + entry.Scrap) * routing.MinutesPerPiece;
            entry.RequiredMinutes = requiredMinutes;
            entry.EfficiencyForOperation = CalculateEfficiency(requiredMinutes, entry.WorkedMinutes);
        }

        private decimal CalculateEfficiency(decimal requiredMinutes, decimal workedMinutes)
        {
            if (workedMinutes <= 0) return 0m;

            return (requiredMinutes / workedMinutes) * 100m;
        }

        async Task IEntryService.SetEfficiencyAsync(Entry entry)
        {
            await SetEfficiencyAsync(entry);
        }

        public Task<string> Greetings(Entry entry)
        {
            return _greetingService.GetGreetingMessageAsync(entry);
        }
    }
}
