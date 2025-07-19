using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Helpers
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = [];

        public void Add(string errorMessage)
        {
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                Errors.Add(errorMessage.Trim());
            }
        }

        public void AddRange(IEnumerable<string> messages)
        {
            foreach (var msg in messages)
            {
                Add(msg);
            }
        }
    }

    public class EntryValidator
    {
        private readonly EfficiencyTrackDbContext _context;

        public EntryValidator(EfficiencyTrackDbContext context)
        {
            _context = context;
        }

        public async Task<ValidationResult> ValidateAsync(Entry entry)
        {
            ValidationResult result = new();

            if (await IsDuplicateEntry(entry))
            {
                result.Add("Вече сте добавили тези данни през днешния ден. МОЛЯ НЕ ПРАВЕТЕ ДВОЙНИ ЗАПИСИ В СИСТЕМАТА.");
            }

            result.AddRange((await ValidateShiftTimeAsync(entry)).Errors);
            result.AddRange((await ValidateEfficiencyAsync(entry)).Errors);

            return result;
        }

        private async Task<bool> IsDuplicateEntry(Entry entry)
        {
            return await _context.Entries.AsNoTracking().AnyAsync(x =>
                x.Date.Date == entry.Date.Date &&
                x.EmployeeId == entry.EmployeeId &&
                x.RoutingId == entry.RoutingId &&
                x.Pieces == entry.Pieces &&
                x.WorkedMinutes == entry.WorkedMinutes &&
                !x.IsDeleted);
        }

        private async Task<ValidationResult> ValidateShiftTimeAsync(Entry entry)
        {
            ValidationResult result = new();

            var shift = await _context.Shifts.AsNoTracking().FirstOrDefaultAsync(s => s.Id == entry.ShiftId);
            if (shift == null)
            {
                result.Add("Невалидна смяна (Shift).");
                return result;
            }

            int workedSoFar = (int)await _context.Entries
                .AsNoTracking()
                .Where(x => x.EmployeeId == entry.EmployeeId && x.Date.Date == entry.Date.Date && !x.IsDeleted)
                .SumAsync(x => x.WorkedMinutes);

            int totalWithCurrent = (int)(workedSoFar + entry.WorkedMinutes);

            if (totalWithCurrent > shift.DurationMinutes)
            {
                int remaining = shift.DurationMinutes - workedSoFar;

                result.Add(remaining > 0
                    ? $"Превишено време! Можете да добавите до {remaining} минути за тази смяна."
                    : $"Вече сте изчерпали всички {shift.DurationMinutes} минути от смяната.");
            }

            return result;
        }

        private async Task<ValidationResult> ValidateEfficiencyAsync(Entry entry)
        {
            ValidationResult result = new();

            var routing = await _context.Routings.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == entry.RoutingId);

            if (routing == null)
            {
                result.Add("Невалиден RoutingId при проверка на ефективността.");
                return result;
            }

            int totalPieces = entry.Pieces + entry.Scrap;

            if (entry.WorkedMinutes <= 0 || totalPieces <= 0)
            {
                result.Add("Имате грешка при попълване на данните. Проверете въведените бройки и минути.");
                return result;
            }

            decimal requiredMinutes = totalPieces * routing.MinutesPerPiece;
            decimal efficiency = requiredMinutes / entry.WorkedMinutes * 100;

            if (efficiency > 150)
            {
                result.Add("Изчислената ефективност е прекалено висока. Проверете отново въведените бройки и минути.");
            }

            return result;
        }
    }
}
