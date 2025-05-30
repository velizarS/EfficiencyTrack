using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Data.Data
{
    public class EfficiencyTrackDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Entry> Entries { get; set; } = null!;
        public DbSet<Routing> Routings { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<DailyEfficiency> DailyEfficiencies { get; set; } = null!;

        public EfficiencyTrackDbContext(DbContextOptions<EfficiencyTrackDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ModelConfiguration.ConfigureModels(modelBuilder);
        }
    }
}
