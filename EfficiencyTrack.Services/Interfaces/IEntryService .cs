using EfficiencyTrack.Data.Models;

public interface IEntryService : ICrudService<Entry>
{
    Task SetEfficiencyAsync(Entry entry);
    Task<Entry?> GetByIdWithIncludesAsync(Guid id);
    Task<List<Entry>> GetAllWithIncludesAsync();
    Task<string> Greetings(Entry entry);
    IQueryable<Entry> GetFilteredEntries(string? searchTerm, string? sortBy, bool sortAsc);
}
