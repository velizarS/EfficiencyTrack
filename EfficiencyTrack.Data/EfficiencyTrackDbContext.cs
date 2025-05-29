using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Data
{
    public class EfficiencyTrackDbContext : IdentityDbContext
    {
        public EfficiencyTrackDbContext(DbContextOptions<EfficiencyTrackDbContext> options)
            : base(options)
        {
        }
    }
}
