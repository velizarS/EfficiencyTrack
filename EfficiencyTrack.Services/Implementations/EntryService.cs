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
        private readonly EntryValidator _validator;

        public EntryService(
            EfficiencyTrackDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IDailyEfficiencyService dailyEfficiencyService)
            : base(context, httpContextAccessor)
        {
            _dailyEfficiencyService = dailyEfficiencyService ?? throw new ArgumentNullException(nameof(dailyEfficiencyService));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _validator = new EntryValidator(_context);
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
            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, validationResult.Errors));
            }

            await SetEfficiencyAsync(entity);
            await base.AddAsync(entity);
            await _dailyEfficiencyService.UpdateDailyEfficiencyAsync(entity.EmployeeId, entity.Date);
        }

        public override async Task UpdateAsync(Entry entity)
        {
            var validationResult = await _validator.ValidateAsync(entity);
            if (!validationResult.IsValid)
            {
                throw new InvalidOperationException(string.Join(Environment.NewLine, validationResult.Errors));
            }

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
                return 0; // Без изключение, просто връщаме 0

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

        // Ако искаш, можеш да оставиш това като част от интерфейса:
        async Task IEntryService.SetEfficiencyAsync(Entry entry)
        {
            await SetEfficiencyAsync(entry);
        }

        public async Task<string> Greetings(Entry entry)
        {
            var todayEntries = await _context.Entries
                .AsNoTracking()
                .Where(x => x.EmployeeId == entry.EmployeeId && x.Date.Date == DateTime.UtcNow.Date)
                .ToListAsync();

            string message = "Успешен запис!\n";

            if (todayEntries.Count == 1)
            {
                var workerName = (await _context.Employees
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == entry.EmployeeId))?.FirstName;

                if (!string.IsNullOrEmpty(workerName))
                {
                    message += $"Здравейте, {workerName}!\n";
                    message += "Пожелавам ви лека работа и успешен ден!\n";
                }
            }

            if (entry.EfficiencyForOperation >= 100)
            {
                message += "Представянето надминава очакванията ни със супер резултатите Ви. Благодарим!\n";
            }
            else if (entry.EfficiencyForOperation >= 90)
            {
                message += "Представянето оправдава очакванията ни с добрите Ви резултати. Благодарим!\n";
            }
            else if (entry.EfficiencyForOperation >= 85)
            {
                message += "Представянето частично отговаря на очакванията ни, необходимо е подобрение.\nС какво можем да Ви помогнем? Обърнете се към прекия Ви ръководител!\n";
            }
            else
            {
                message += "Незадоволително представяне.\nНеобходими са навременни коригиращи действия.\nС какво можем да Ви помогнем? Обърнете се към прекия Ви ръководител!\n";
            }

            return message;
        }
    }
}
