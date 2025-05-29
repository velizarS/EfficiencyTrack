using EfficiencyTrack.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Data
{
    public class EfficiencyTrackDbContext : IdentityDbContext<ApllicationUser, IdentityRole<Guid>, Guid>
    {
        public EfficiencyTrackDbContext(DbContextOptions<EfficiencyTrackDbContext> options)
            : base(options)
        {
        }
    }
}
