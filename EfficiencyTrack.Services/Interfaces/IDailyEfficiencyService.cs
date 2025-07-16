using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.DTOs.EfficiencyTrack.Services.DTOs;

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
