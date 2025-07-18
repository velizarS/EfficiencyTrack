using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Helpers
{
    public class ValidationResult
    {
        public bool IsValid => Errors.Count == 0;
        public List<string> Errors { get; } = [];
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
                result.Errors.Add("Вече сте добавили тези данни през днешния ден. МОЛЯ НЕ ПРАВЕТЕ ДВОЙНИ ЗАПИСИ В СИСТЕМАТА.");
            }

            ValidationResult shiftValidation = await ValidateShiftTimeAsync(entry);
            result.Errors.AddRange(shiftValidation.Errors);

            ValidationResult efficiencyValidation = await ValidateEfficiencyAsync(entry);
            result.Errors.AddRange(efficiencyValidation.Errors);

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

            Shift? shift = await _context.Shifts.AsNoTracking().FirstOrDefaultAsync(s => s.Id == entry.ShiftId);
            if (shift == null)
            {
                result.Errors.Add("Невалидна смяна (Shift).");
                return result;
            }

            int totalWorkedMinutes = (int)await _context.Entries
                .AsNoTracking()
                .Where(x => x.EmployeeId == entry.EmployeeId && x.Date.Date == entry.Date.Date && !x.IsDeleted)
                .SumAsync(x => x.WorkedMinutes);

            int remainingMinutes = shift.DurationMinutes - totalWorkedMinutes;

            if (entry.WorkedMinutes > remainingMinutes)
            {
                string message = remainingMinutes > 0
                    ? $"Въведеното изработено време надвишава максимално допустимото. Оставащи минути за въвеждане: {remainingMinutes}."
                    : $"Въведеното изработено време е повече от времето на работната смяна ({shift.DurationMinutes} минути).";

                result.Errors.Add(message);
            }

            return result;
        }

        private async Task<ValidationResult> ValidateEfficiencyAsync(Entry entry)
        {
            ValidationResult result = new();

            Routing? routing = await _context.Routings.AsNoTracking().FirstOrDefaultAsync(r => r.Id == entry.RoutingId);
            if (routing == null)
            {
                result.Errors.Add("Невалиден RoutingId при проверка на ефективността.");
                return result;
            }

            int totalPieces = entry.Pieces + entry.Scrap;
            if (entry.WorkedMinutes <= 0 || totalPieces <= 0)
            {
                result.Errors.Add("Имате грешка при попълване на данните, проверете въведените бройки и минути.");
                return result;
            }

            decimal requiredMinutes = totalPieces * routing.MinutesPerPiece;
            decimal efficiency = requiredMinutes / entry.WorkedMinutes * 100;

            if (efficiency > 150)
            {
                result.Errors.Add("Имате грешка при попълване на данните, проверете въведените бройки и минути.");
            }


            requiredMinutes = totalPieces * routing.MinutesPerPiece;
            efficiency = requiredMinutes / entry.WorkedMinutes * 100;

            if (efficiency > 150)
            {
                result.Errors.Add("Имате грешка при попълване на данните, проверете въведените бройки и минути.");
            }

            return result;
        }
    }
}
