using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;

public interface IEntryService : ICrudService<Entry>
{
    Task SetEfficiencyAsync(Entry entry);
    Task<Entry?> GetByIdWithIncludesAsync(Guid id);
    Task<List<Entry>> GetAllWithIncludesAsync();
}
