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
            {
                throw new ArgumentException("Invalid ID provided.", nameof(id));
            }

            DailyEfficiency? dailyEfficiency = await _context.DailyEfficiencies
                .Include(de => de.Employee)
                .Include(de => de.Shift)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (dailyEfficiency == null)
            {
                return null;
            }

            List<Entry> entries = await _context.Entries
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
                    EfficiencyForOperation = e.EfficiencyForOperation
                }).ToList()
            };
        }

        public async Task<IEnumerable<TopEfficiencyDto>> GetTop10ForThisMonthAsync()
        {
            DateTime now = DateTime.UtcNow;
            int currentMonth = now.Month;
            int currentYear = now.Year;

            var topEfficiencies = await _context.DailyEfficiencies
                .AsNoTracking()
                .Where(e => e.Date.Month == currentMonth &&
                            e.Date.Year == currentYear &&
                            e.EfficiencyPercentage >= EfficiencyAppConstants.MinimumEfficiencyForTopList &&
                            !e.IsDeleted)
                .GroupBy(e => e.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    AverageEfficiency = g.Average(e => e.EfficiencyPercentage),
                    LatestEntry = g.OrderByDescending(e => e.Date).FirstOrDefault()
                })
                .OrderByDescending(g => g.AverageEfficiency)
                .Take(10)
                .ToListAsync();

            List<TopEfficiencyDto> result = [];

            foreach (var group in topEfficiencies)
            {
                if (group.LatestEntry == null)
                {
                    continue;
                }

                Employee? emp = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.ShiftManagerUser)
                    .FirstOrDefaultAsync(e => e.Id == group.EmployeeId);

                result.Add(new TopEfficiencyDto
                {
                    FullName = emp.FirstName + " " + emp.LastName,
                    EfficiencyPercentage = group.AverageEfficiency,
                    DepartmentName = emp.Department.Name,
                    ShiftManagerName = emp.ShiftManagerUser.UserName,
                    ShiftName = group.LatestEntry.Shift?.Name
                });
            }

            return result;
        }

        public async Task<IEnumerable<TopEfficiencyDto>> GetTop10ForTodayAsync()
        {
            DateTime today = DateTime.UtcNow.Date;

            return await _context.DailyEfficiencies
                .AsNoTracking()
                .Where(e => e.Date.Date == today &&
                            e.EfficiencyPercentage >= EfficiencyAppConstants.MinimumEfficiencyForTopList &&
                            !e.IsDeleted)
                .Include(de => de.Employee)
                    .ThenInclude(emp => emp.Department)
                .Include(de => de.Employee)
                    .ThenInclude(emp => emp.ShiftManagerUser)
                .Include(de => de.Shift)
                .OrderByDescending(e => e.EfficiencyPercentage)
                .Take(10)
                .Select(e => new TopEfficiencyDto
                {
                    FullName = e.Employee.FirstName + " " + e.Employee.LastName,
                    EfficiencyPercentage = e.EfficiencyPercentage,
                    DepartmentName = e.Employee.Department.Name,
                    ShiftManagerName = e.Employee.ShiftManagerUser.UserName,
                    ShiftName = e.Shift.Name
                })
                .ToListAsync();
        }

        public async Task UpdateDailyEfficiencyAsync(Guid employeeId, DateTime date)
        {
            DateTime dayStart = date.Date;
            DateTime dayEnd = dayStart.AddDays(1);

            List<Entry> entriesOfDay = await _context.Entries
                .AsNoTracking()
                .Where(e => e.EmployeeId == employeeId &&
                            e.Date >= dayStart &&
                            e.Date < dayEnd &&
                            !e.IsDeleted)
                .ToListAsync();

            DailyEfficiency? existing = await _context.DailyEfficiencies
                .FirstOrDefaultAsync(de => de.EmployeeId == employeeId &&
                                           de.Date >= dayStart && de.Date < dayEnd);

            if (!entriesOfDay.Any())
            {
                if (existing != null)
                {
                    _ = _context.DailyEfficiencies.Remove(existing);
                    _ = await _context.SaveChangesAsync();
                }
                return;
            }

            List<Guid> distinctShiftIds = entriesOfDay.Select(e => e.ShiftId).Distinct().ToList();
            if (distinctShiftIds.Count != 1)
            {
                throw new InvalidOperationException("Entries span multiple shifts. Cannot calculate efficiency.");
            }

            Guid shiftId = distinctShiftIds.First();

            int shiftDuration = await _context.Shifts
                .AsNoTracking()
                .Where(s => s.Id == shiftId)
                .Select(s => s.DurationMinutes)
                .FirstOrDefaultAsync();

            if (shiftDuration <= 0)
            {
                throw new InvalidOperationException("Invalid or missing shift duration.");
            }

            IEnumerable<Guid> routingIds = entriesOfDay.Select(e => e.RoutingId).Distinct();
            Dictionary<Guid, decimal> routingMap = await _context.Routings
                .AsNoTracking()
                .Where(r => routingIds.Contains(r.Id) && !r.IsDeleted)
                .ToDictionaryAsync(r => r.Id, r => r.MinutesPerPiece);

            int totalNeededMinutes = (int)entriesOfDay.Sum(e =>
                routingMap.TryGetValue(e.RoutingId, out decimal minutesPerPiece)
                    ? (e.Pieces + e.Scrap) * minutesPerPiece
                    : 0);

            decimal totalWorkedMinutes = entriesOfDay.Sum(e => e.WorkedMinutes);

            decimal efficiencyPercent = shiftDuration > 0
                ? (decimal)totalNeededMinutes / shiftDuration * 100
                : 0;

            if (existing == null)
            {
                DailyEfficiency dailyEfficiency = new()
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employeeId,
                    Date = dayStart,
                    ShiftId = shiftId,
                    EfficiencyPercentage = efficiencyPercent,
                    TotalWorkedMinutes = totalWorkedMinutes,
                    TotalNeededMinutes = totalNeededMinutes,
                    ComputedOn = DateTime.UtcNow
                };
                _ = _context.DailyEfficiencies.Add(dailyEfficiency);
            }
            else
            {
                existing.ShiftId = shiftId;
                existing.TotalWorkedMinutes = totalWorkedMinutes;
                existing.TotalNeededMinutes = totalNeededMinutes;
                existing.EfficiencyPercentage = efficiencyPercent;
                existing.ComputedOn = DateTime.UtcNow;
            }

            _ = await _context.SaveChangesAsync();
        }
    }
}
