using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.DTOs.EfficiencyTrack.Services.DTOs;
using EfficiencyTrack.Services.DTOs;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Implementations
{
    public class DailyEfficiencyService : CrudService<DailyEfficiency>, IDailyEfficiencyService
    {
        private readonly EfficiencyTrackDbContext _context;

        public DailyEfficiencyService(
            EfficiencyTrackDbContext context,
            IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            _context = context;
        }

        public async Task<DailyEfficiencyDto?> GetDailyEfficiencyDtoAsync(Guid employeeId, DateTime date)
        {
            var dailyEfficiency = await _context.DailyEfficiencies
                .AsNoTracking()
                .Include(de => de.Employee)
                .Include(de => de.Shift)
                .FirstOrDefaultAsync(de => de.EmployeeId == employeeId && de.Date.Date == date.Date && !de.IsDeleted);

            if (dailyEfficiency == null)
                return null;

            var entries = await _context.Entries
                .AsNoTracking()
                .Include(e => e.Routing)
                .Where(e => e.EmployeeId == employeeId && e.Date.Date == date.Date && !e.IsDeleted)
                .ToListAsync();

            return new DailyEfficiencyDto
            {
                Id = dailyEfficiency.Id,
                Date = dailyEfficiency.Date,
                EmployeeCode = dailyEfficiency.Employee?.Code ?? "N/A",
                EmployeeFullName = string.Join(" ", dailyEfficiency.Employee?.FirstName, dailyEfficiency.Employee?.MiddleName, dailyEfficiency.Employee?.LastName).Trim(),
                TotalWorkedMinutes = dailyEfficiency.TotalWorkedMinutes,
                ShiftName = dailyEfficiency.Shift?.Name ?? "N/A",
                EfficiencyPercentage = dailyEfficiency.EfficiencyPercentage,
                Entries = entries.Select(e => new EntryDto
                {
                    Date = e.Date,
                    EmployeeId = e.EmployeeId,
                    RoutingId = e.RoutingId,
                    RoutingName = e.Routing?.Code,
                    EfficiencyForOperation = e.RequiredMinutes
                }).ToList()
            };
        }


        public async Task<decimal> CalculateDailyEfficiencyAsync(Guid employeeId, DateTime date)
        {
            var entriesOfDay = await _context.Entries
                .AsNoTracking()
                .Where(e => e.EmployeeId == employeeId
                         && e.Date.Date == date.Date
                         && !e.IsDeleted)
                .ToListAsync();

            if (!entriesOfDay.Any())
            {
                return 0m;
            }

            decimal totalNeededMinutes = entriesOfDay
                .Sum(e => e.RequiredMinutes);

            var shiftId = entriesOfDay.First().ShiftId;

            var shiftDuration = await _context.Shifts
                .AsNoTracking()
                .Where(s => s.Id == shiftId)
                .Select(s => s.DurationMinutes)
                .FirstOrDefaultAsync();

            if (shiftDuration == 0)
            {
                return 0m;
            }

            decimal efficiencyPercent = (totalNeededMinutes / shiftDuration) * 100;

            return efficiencyPercent;
        }
    }
}
