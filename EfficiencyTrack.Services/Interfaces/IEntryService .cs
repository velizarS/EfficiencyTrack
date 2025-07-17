using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEntryService : ICrudService<Entry>
{
    Task SetEfficiencyAsync(Entry entry);
    Task<Entry?> GetByIdWithIncludesAsync(Guid id);
    Task<List<Entry>> GetAllWithIncludesAsync();
    Task<string> Greetings(Entry entry);
}
