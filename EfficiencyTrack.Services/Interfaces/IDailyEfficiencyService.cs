using EfficiencyTrack.Data.Models;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IDailyEfficiencyService : ICrudService<DailyEfficiency>
    {
        Task<decimal> CalculateDailyEfficiencyAsync(Guid employeeId, DateTime date);
    }
}
