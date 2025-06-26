using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;

public interface IEntryService : ICrudService<Entry>
{
    Task SetEfficiencyAsync(Entry entry);
}
