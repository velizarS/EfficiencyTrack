using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Implementations
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = new();
    }

    public class EntryValidator
    {
        private readonly EfficiencyTrackDbContext _context;

        public EntryValidator(EfficiencyTrackDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<ValidationResult> ValidateAsync(Entry entry)
        {
            var result = new ValidationResult();

            if (await IsDuplicateEntry(entry))
            {
                result.Errors.Add("Вече сте добавили тези данни през днешния ден. МОЛЯ НЕ ПРАВЕТЕ ДВОЙНИ ЗАПИСИ В СИСТЕМАТА.");
            }

            var (isOver, remainingMinutes, shiftDuration) = await IsOverShiftTimeAsync(entry);
            if (isOver)
            {
                string message = remainingMinutes > 0
                    ? $"Въведеното изработено време надвишава максимално допустимото. Оставащи минути за въвеждане: {remainingMinutes}."
                    : $"Въведеното изработено време е повече от времето на работната смяна ({shiftDuration} минути).";

                result.Errors.Add(message);
            }

            if (await IsOverEfficiencyPercentAsync(entry))
            {
                result.Errors.Add("Имате грешка при попълване на данните, проверете въведените бройки и минути.");
            }

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

        private async Task<(bool IsOver, int RemainingMinutes, int ShiftDuration)> IsOverShiftTimeAsync(Entry entry)
        {
            // Взимаме shift duration + сумата на изработените минути за деня в една заявка
            var shift = await _context.Shifts.AsNoTracking().FirstOrDefaultAsync(s => s.Id == entry.ShiftId);
            if (shift == null)
                throw new InvalidOperationException("Невалидна смяна (Shift).");

            decimal totalWorkedMinutes = await _context.Entries
                .AsNoTracking()
                .Where(x => x.EmployeeId == entry.EmployeeId && x.Date.Date == entry.Date.Date && !x.IsDeleted)
                .SumAsync(x => x.WorkedMinutes);

            int remainingMinutes = shift.DurationMinutes - (int)totalWorkedMinutes;

            bool isOver = (totalWorkedMinutes + entry.WorkedMinutes) > shift.DurationMinutes;

            return (isOver, remainingMinutes, shift.DurationMinutes);
        }

        private async Task<bool> IsOverEfficiencyPercentAsync(Entry entry)
        {
            var routing = await _context.Routings.AsNoTracking().FirstOrDefaultAsync(r => r.Id == entry.RoutingId);
            if (routing == null)
                throw new InvalidOperationException("Невалиден RoutingId при проверка на ефективността.");

            var requiredMinutes = (entry.Pieces + entry.Scrap) * routing.MinutesPerPiece;

            if (entry.WorkedMinutes <= 0)
                return true; // невалидно време, значи надвишава

            var efficiency = (requiredMinutes / entry.WorkedMinutes) * 100;

            return efficiency > 150;
        }
    }
}
