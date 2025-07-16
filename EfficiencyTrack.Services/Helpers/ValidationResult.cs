using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Helpers
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
            _context = context;
        }

        public async Task<ValidationResult> ValidateAsync(Entry entry)
        {
            var result = new ValidationResult();

            if (await IsDuplicateEntry(entry))
            {
                result.Errors.Add("Вече сте добавили тези данни през днешния ден. МОЛЯ НЕ ПРАВЕТЕ ДВОЙНИ ЗАПИСИ В СИСТЕМАТА.");
            }

            var shiftValidation = await ValidateShiftTimeAsync(entry);
            result.Errors.AddRange(shiftValidation.Errors);

            var efficiencyValidation = await ValidateEfficiencyAsync(entry);
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
            var result = new ValidationResult();

            var shift = await _context.Shifts.AsNoTracking().FirstOrDefaultAsync(s => s.Id == entry.ShiftId);
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
            var result = new ValidationResult();

            var routing = await _context.Routings.AsNoTracking().FirstOrDefaultAsync(r => r.Id == entry.RoutingId);
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
<<<<<<< HEAD:EfficiencyTrack.Services/Helpers/ValidationResult.cs

            var requiredMinutes = totalPieces * routing.MinutesPerPiece;
            var efficiency = requiredMinutes / entry.WorkedMinutes * 100;

            if (efficiency > 150)
            {
                result.Errors.Add("Имате грешка при попълване на данните, проверете въведените бройки и минути.");
            }

=======

            var requiredMinutes = totalPieces * routing.MinutesPerPiece;
            var efficiency = (requiredMinutes / entry.WorkedMinutes) * 100;

            if (efficiency > 150)
            {
                result.Errors.Add("Имате грешка при попълване на данните, проверете въведените бройки и минути.");
            }

>>>>>>> a1a4673be72f9c81ce7a9985c64bba5dde972ddc:EfficiencyTrack.Services/Implementations/ValidationResult.cs
            return result;
        }
    }
}
