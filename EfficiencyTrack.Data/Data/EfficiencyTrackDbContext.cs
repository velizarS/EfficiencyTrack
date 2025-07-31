using EfficiencyTrack.Data.Configuration;
using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Data.Data
{
    public class EfficiencyTrackDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<Entry> Entries { get; set; } = null!;
        public DbSet<Routing> Routings { get; set; } = null!;
        public DbSet<Shift> Shifts { get; set; } = null!;
        public DbSet<DailyEfficiency> DailyEfficiencies { get; set; } = null!;
        public DbSet<Feedback> Feedbacks { get; set; } = null!;
        public DbSet<SentEfficiencyReport> SentEfficiencyReports { get; set; }

        public EfficiencyTrackDbContext(
            DbContextOptions<EfficiencyTrackDbContext> options,
            IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override int SaveChanges()
        {
            ApplyAuditInformation();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInformation();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInformation()
        {
            string userName = "Unknown";

            try
            {
                userName = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? "Unknown";
            }
            catch
            {
                
            }
            IEnumerable<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<BaseEntity>> entries = ChangeTracker.Entries<BaseEntity>();

            foreach (var entry in entries)
            {
                if (entry.Entity == null)
                    continue;  

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = DateTime.UtcNow;
                        entry.Entity.CreatedBy = userName;
                        entry.Entity.IsDeleted = false;
                        break;

                    case EntityState.Modified:
                        entry.Entity.ModifiedOn = DateTime.UtcNow;
                        entry.Entity.ModifiedBy = userName;
                        break;

                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedBy = userName;
                        break;
                }
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ModelConfiguration.ConfigureModels(modelBuilder);
        }
    }
}
