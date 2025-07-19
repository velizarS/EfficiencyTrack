using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.DTOs;
using EfficiencyTrack.Services.DTOs.EfficiencyTrack.Services.DTOs;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IDailyEfficiencyService
    {
        Task UpdateDailyEfficiencyAsync(Guid employeeId, DateTime date);
        Task<IEnumerable<DailyEfficiency>> GetAllAsync();
        Task<IEnumerable<DailyEfficiency>> GetByShiftManagerIdAsync(Guid shiftManagerUserId);
        Task<IEnumerable<DailyEfficiency>> GetByEmployeeCodeAsync(string employeeCode);
        Task<DailyEfficiencyDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<TopEfficiencyDto>> GetTop10ForTodayAsync();
        Task<IEnumerable<TopEfficiencyDto>> GetTop10ForThisMonthAsync();
    }
}
