using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;

public interface IEntryService : ICrudService<Entry>
{
    public void SetEfficiency(Entry entry);
}
