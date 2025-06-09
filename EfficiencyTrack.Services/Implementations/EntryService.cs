using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using Microsoft.AspNetCore.Http;

namespace EfficiencyTrack.Services.Implementations
{
    public class EntryService : CrudService<Entry>, IEntryService
    {
        public EntryService(EfficiencyTrackDbContext context, IHttpContextAccessor httpContextAccessor)
        : base(context, httpContextAccessor)
        {
        }

        public void SetEfficiency(Entry entry)
        {
            entry.EfficiencyForOperation = Entry.CalculateEfficiencyForOperation(entry.RequiredMinutes, entry.WorkedMinutes);
        }
    }
}
