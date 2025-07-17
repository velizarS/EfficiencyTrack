using Common;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.DTOs;
using EfficiencyTrack.Services.DTOs.EfficiencyTrack.Services.DTOs;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Implementations
{
    public class DailyEfficiencyService : IDailyEfficiencyService
    {
        private readonly EfficiencyTrackDbContext _context;

        public DailyEfficiencyService(EfficiencyTrackDbContext context)
        {
            _context = context;
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
                .Where(e => e.EmployeeId == dailyEfficiency.EmployeeId &&
                            e.Date.Date == dailyEfficiency.Date.Date &&
                            !e.IsDeleted)
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

        public async Task<IEnumerable<DailyEfficiency>> GetTop10ForThisMonthAsync()
        {
            var now = DateTime.UtcNow;

            var topEfficiencies = await _context.DailyEfficiencies
                .AsNoTracking()
                .Where(e => e.Date.Month == now.Month &&
                            e.Date.Year == now.Year &&
                            e.EfficiencyPercentage >= EfficiencyAppConstants.MinimumEfficiencyForTopList &&
                            !e.IsDeleted)
                .GroupBy(e => e.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    AverageEfficiency = g.Average(e => e.EfficiencyPercentage),
                    Entries = g.OrderByDescending(e => e.Date).ToList()
                })
                .OrderByDescending(g => g.AverageEfficiency)
                .Take(10)
                .ToListAsync();

            var result = new List<DailyEfficiency>();

            foreach (var group in topEfficiencies)
            {
                var latest = group.Entries.First();

                result.Add(new DailyEfficiency
                {
                    Id = latest.Id, 
                    Date = latest.Date,
                    Employee = await _context.Employees
                        .Include(e => e.Department)
                        .Include(e => e.ShiftManagerUser)
                        .FirstOrDefaultAsync(e => e.Id == group.EmployeeId),
                    EfficiencyPercentage = group.AverageEfficiency,
                    Shift = latest.Shift,
                    TotalNeededMinutes = latest.TotalNeededMinutes,
                    TotalWorkedMinutes = latest.TotalWorkedMinutes
                });
            }
            return result;
        }

        public async Task<IEnumerable<DailyEfficiency>> GetTop10ForTodayAsync()
        {
            var today = DateTime.UtcNow.Date;

            var topTen = await _context.DailyEfficiencies
                .AsNoTracking()
                .Where(e =>
                    e.Date.Date == today.Date &&
                    e.EfficiencyPercentage >= EfficiencyAppConstants.MinimumEfficiencyForTopList &&
                    !e.IsDeleted)
                .Include(de => de.Employee)
                    .ThenInclude(emp => emp.Department)
                .Include(de => de.Employee)
                    .ThenInclude(emp => emp.ShiftManagerUser)
                .Include(de => de.Shift)
                .OrderByDescending(e => e.EfficiencyPercentage)
                .Take(10)
                .ToListAsync();

            return topTen;
        }





        public async Task UpdateDailyEfficiencyAsync(Guid employeeId, DateTime date)
        {
            var entriesOfDay = await _context.Entries.AsNoTracking()
                .Where(e => e.EmployeeId == employeeId &&
                            e.Date.Date == date.Date &&
                            !e.IsDeleted)
                .ToListAsync();

            var existing = await _context.DailyEfficiencies
                .FirstOrDefaultAsync(de => de.EmployeeId == employeeId &&
                                           de.Date.Date == date.Date);

            if (!entriesOfDay.Any())
            {
                if (existing != null)
                {
                    _context.DailyEfficiencies.Remove(existing);
                    await _context.SaveChangesAsync();
                }
                return;
            }

            var routingIds = entriesOfDay.Select(e => e.RoutingId).Distinct();
            var routingMap = await _context.Routings
                .AsNoTracking()
                .Where(r => routingIds.Contains(r.Id) && !r.IsDeleted)
                .ToDictionaryAsync(r => r.Id, r => r.MinutesPerPiece);

            int totalNeededMinutes = (int)entriesOfDay.Sum(e =>
                routingMap.TryGetValue(e.RoutingId, out var minutesPerPiece)
                    ? (e.Pieces + e.Scrap) * minutesPerPiece
                    : 0);

            var shiftId = entriesOfDay.First().ShiftId;

            var shiftDuration = await _context.Shifts
                .AsNoTracking()
                .Where(s => s.Id == shiftId)
                .Select(s => s.DurationMinutes)
                .FirstOrDefaultAsync();

            if (shiftDuration == 0)
                throw new InvalidOperationException("Shift duration is 0 or shift not found.");

            decimal efficiencyPercent = (totalNeededMinutes / shiftDuration) * 100;
            decimal totalTime = entriesOfDay.Sum(e => e.WorkedMinutes);
            if (existing == null)
            {
                var dailyEfficiency = new DailyEfficiency
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
                existing.EfficiencyPercentage = efficiencyPercent;
                existing.ComputedOn = DateTime.UtcNow;
                existing.ShiftId = shiftId;
                existing.TotalWorkedMinutes = totalTime;
                existing.TotalNeededMinutes = totalNeededMinutes;
                _context.DailyEfficiencies.Update(existing);
            }

            await _context.SaveChangesAsync();
        }
    }
}
