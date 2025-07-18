using EfficiencyTrack.Data.Models;

public interface IEntryService : ICrudService<Entry>
{
    Task SetEfficiencyAsync(Entry entry);
    Task<Entry?> GetByIdWithIncludesAsync(Guid id);
    Task<List<Entry>> GetAllWithIncludesAsync();
    Task<string> Greetings(Entry entry);
}
