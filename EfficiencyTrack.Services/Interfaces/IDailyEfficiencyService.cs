using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.DTOs;
using EfficiencyTrack.Services.DTOs.EfficiencyTrack.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IDailyEfficiencyService
    {
        Task UpdateDailyEfficiencyAsync(Guid employeeId, DateTime date);
        Task<IEnumerable<DailyEfficiency>> GetAllAsync();
        Task<DailyEfficiencyDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<DailyEfficiency>> GetTop10ForTodayAsync();
        Task<IEnumerable<DailyEfficiency>> GetTop10ForThisMonthAsync();
    }
}
