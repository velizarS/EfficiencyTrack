using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Data.Configuration
{
    public static class ModelConfiguration
    {
        public static void ConfigureModels(ModelBuilder modelBuilder)
        {
            _ = modelBuilder.Entity<Shift>()
                .HasIndex(sh => sh.Name)
                .IsUnique();

            _ = modelBuilder.Entity<Department>()
                .HasIndex(d => d.Name)
                .IsUnique();

            _ = modelBuilder.Entity<Routing>()
                .HasIndex(r => r.Code)
                .IsUnique();

            _ = modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Code)
                .IsUnique();

            _ = modelBuilder.Entity<Entry>()
                .HasIndex(e => new { e.Date, e.EmployeeId, e.RoutingId, e.ShiftId, e.Pieces, e.WorkedMinutes })
                .IsUnique()
                .HasDatabaseName("IX_Entries_Date_Employee_Routing_Shift");

            _ = modelBuilder.Entity<DailyEfficiency>()
                .HasIndex(e => new { e.Date, e.EmployeeId })
                .IsUnique()
                .HasDatabaseName("IX_DailyEfficiency_Date_Employee");


            _ = modelBuilder.Entity<Shift>()
                .HasMany(s => s.Entries)
                .WithOne(e => e.Shift)
                .OnDelete(DeleteBehavior.Restrict);

            _ = modelBuilder.Entity<Department>()
               .HasMany(d => d.Employees)
               .WithOne(e => e.Department)
               .OnDelete(DeleteBehavior.Restrict);

            _ = modelBuilder.Entity<Routing>()
                .HasMany(r => r.Entries)
                .WithOne(e => e.Routing)
                .OnDelete(DeleteBehavior.Restrict);

            _ = modelBuilder.Entity<Employee>()
                .HasMany(e => e.Entries)
                .WithOne(e => e.Employee)
                .OnDelete(DeleteBehavior.Restrict);

            _ = modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Employees)
                .WithOne(e => e.ApplicationUser)
                .HasForeignKey(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            _ = modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Claims)
                .WithOne()
                .HasForeignKey(c => c.UserId)
                .IsRequired();

            _ = modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Roles)
                .WithOne()
                .HasForeignKey(r => r.UserId)
                .IsRequired();

            _ = modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Logins)
                .WithOne()
                .HasForeignKey(l => l.UserId)
                .IsRequired();

        }
    }
}
