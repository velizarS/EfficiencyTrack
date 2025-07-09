using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.DTOs;
using EfficiencyTrack.Services.DTOs.EfficiencyTrack.Services.DTOs;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Implementations
{
    public class DailyEfficiencyService : IDailyEfficiencyService
    {
        private readonly EfficiencyTrackDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DailyEfficiencyService(
            EfficiencyTrackDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<DailyEfficiency>> GetAllAsync()
        {
            return await _context.DailyEfficiencies
                .AsNoTracking()
                .Where(e => !e.IsDeleted)
                .Include(de => de.Employee)
                .Include(de => de.Shift)
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<DailyEfficiencyDto?> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid ID provided.", nameof(id));

            var dailyEfficiency = await _context.DailyEfficiencies
                .Include(de => de.Employee)
                .Include(de => de.Shift)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (dailyEfficiency == null)
                return null;

            var entries = await _context.Entries
                .AsNoTracking()
                .Include(e => e.Routing)
                .Where(e => e.EmployeeId == dailyEfficiency.EmployeeId && e.Date.Date == dailyEfficiency.Date.Date && !e.IsDeleted)
                .ToListAsync();

            return new DailyEfficiencyDto
            {
                Id = dailyEfficiency.Id,
                Date = dailyEfficiency.Date,
                EmployeeCode = dailyEfficiency.Employee?.Code ?? "N/A",
                EmployeeFullName = string.Join(" ", dailyEfficiency.Employee?.FirstName, dailyEfficiency.Employee?.MiddleName, dailyEfficiency.Employee?.LastName).Trim(),
                TotalWorkedMinutes = dailyEfficiency.TotalWorkedMinutes,
                TotalNeddedMinutes = dailyEfficiency.TotalNeededMinutes,
                ShiftName = dailyEfficiency.Shift?.Name ?? "N/A",
                EfficiencyPercentage = dailyEfficiency.EfficiencyPercentage,
                Entries = entries.Select(e => new EntryDto
                {
                    Date = e.Date,
                    EmployeeId = e.EmployeeId,
                    RoutingId = e.RoutingId,
                    RoutingName = e.Routing?.Code,
                    Pieces = e.Pieces,
                    WorkedMinutes = e.WorkedMinutes,
                    EfficiencyForOperation = (decimal)e.EfficiencyForOperation
                }).ToList()
            };
        }

        public async Task UpdateDailyEfficiencyAsync(Guid employeeId, DateTime date)
        {
            var entriesOfDay = await _context.Entries.AsNoTracking()
                .Where(e => e.EmployeeId == employeeId && e.Date.Date == date.Date && !e.IsDeleted)
                .ToListAsync();

            if (!entriesOfDay.Any())
            {
                var existing = await _context.DailyEfficiencies
                    .FirstOrDefaultAsync(de => de.EmployeeId == employeeId && de.Date.Date == date.Date);

                if (existing != null)
                {
                    _context.DailyEfficiencies.Remove(existing);
                    await _context.SaveChangesAsync();
                }
                return;
            }

            decimal totalNeededMinutes = 0;
            foreach (var entry in entriesOfDay)
            {
                var routing = await _context.Routings
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == entry.RoutingId && !r.IsDeleted);
                totalNeededMinutes += (entry.Pieces + entry.Scrap) * routing.MinutesPerPiece;
            }

            var shiftId = entriesOfDay.First().ShiftId;

            var shiftDuration = await _context.Shifts
                .AsNoTracking()
                .Where(s => s.Id == shiftId)
                .Select(s => s.DurationMinutes)
                .FirstOrDefaultAsync();

            if (shiftDuration == 0)
                shiftDuration = 1;

            decimal efficiencyPercent = (totalNeededMinutes / shiftDuration) * 100;

            var dailyEfficiency = await _context.DailyEfficiencies.AsNoTracking()
                .FirstOrDefaultAsync(de => de.EmployeeId == employeeId && de.Date.Date == date.Date);
            var totalTime = entriesOfDay.Sum(e => e.WorkedMinutes);
            if (dailyEfficiency == null)
            {
                dailyEfficiency = new DailyEfficiency
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employeeId,
                    Date = date.Date,
                    ShiftId = shiftId,
                    EfficiencyPercentage = efficiencyPercent,
                    TotalWorkedMinutes = totalTime,
                    TotalNeededMinutes = totalNeededMinutes,
                    ComputedOn = DateTime.UtcNow
                };
                _context.DailyEfficiencies.Add(dailyEfficiency);
            }
            else
            {
                dailyEfficiency.EfficiencyPercentage = efficiencyPercent;
                dailyEfficiency.ComputedOn = DateTime.UtcNow;
                dailyEfficiency.ShiftId = shiftId;
                dailyEfficiency.TotalWorkedMinutes = totalTime;
                dailyEfficiency.TotalNeededMinutes = totalNeededMinutes;
                _context.DailyEfficiencies.Update(dailyEfficiency);
            }

            await _context.SaveChangesAsync();
        }
    }
}