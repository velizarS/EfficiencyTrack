using EfficiencyTrack.Data.Models;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IGreetingService
    {
        Task<string> GetGreetingMessageAsync(Entry entry);
    }

}
