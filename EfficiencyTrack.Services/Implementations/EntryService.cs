using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Helpers;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Implementations
{
    public class EntryService : CrudService<Entry>, IEntryService
    {
        private readonly IDailyEfficiencyService _dailyEfficiencyService;
        private readonly EfficiencyTrackDbContext _context;
        private readonly EntryValidator _validator;
        private readonly IGreetingService _greetingService;

        public EntryService(
            EfficiencyTrackDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IDailyEfficiencyService dailyEfficiencyService,
            IGreetingService greetingService)
            : base(context, httpContextAccessor)
        {
            _context = context;
            _dailyEfficiencyService = dailyEfficiencyService;
            _validator = new EntryValidator(_context);
            _greetingService = greetingService;
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

        public override async Task AddAsync(Entry entity)
        {
            await ValidateAndSetEfficiencyAsync(entity);
            await base.AddAsync(entity);
            await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);
        }

        public override async Task UpdateAsync(Entry entity)
        {
            await ValidateAndSetEfficiencyAsync(entity);
            await base.UpdateAsync(entity);
            await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);
        }

        public override async Task DeleteAsync(Guid id)
        {
            Entry? entity = await base.GetByIdAsync(id);
            if (entity != null)
            {
                await base.DeleteAsync(id);
                await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);
            }
        }

        private async Task ValidateAndSetEfficiencyAsync(Entry entry)
        {
            ValidationResult validationResult = await _validator.ValidateAsync(entry);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, validationResult.Errors));
            }

            await SetEfficiencyAsync(entry);
        }

        private async Task SetEfficiencyAsync(Entry entry)
        {
            Routing? routing = await _context.Routings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == entry.RoutingId);

            if (routing == null)
            {
                throw new InvalidOperationException("Невалиден RoutingId");
            }

            decimal requiredMinutes = (entry.Pieces + entry.Scrap) * routing.MinutesPerPiece;
            entry.RequiredMinutes = requiredMinutes;
            entry.EfficiencyForOperation = CalculateEfficiency(requiredMinutes, entry.WorkedMinutes);
        }


        private decimal CalculateEfficiency(decimal requiredMinutes, decimal workedMinutes)
        {
            return workedMinutes <= 0 ? 0 : requiredMinutes / workedMinutes * 100;
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
