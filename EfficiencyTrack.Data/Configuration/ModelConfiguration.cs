using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Data
{
    public static class ModelConfiguration
    {
        public static void ConfigureModels(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Shift>()
                .HasIndex(sh => sh.Name)
                .IsUnique();

            modelBuilder.Entity<Department>()
                .HasIndex(d => d.Name)
                .IsUnique();

            modelBuilder.Entity<Routing>()
                .HasIndex(r => r.Code)
                .IsUnique();

            modelBuilder.Entity<Employee>()
                .HasIndex(e => e.Code)
                .IsUnique();

            modelBuilder.Entity<Entry>()
                .HasIndex(e => new { e.Date, e.EmployeeId, e.RoutingId, e.ShiftId, e.Pieces, e.WorkedMinutes })
                .IsUnique()
                .HasDatabaseName("IX_Entries_Date_Employee_Routing_Shift");

            modelBuilder.Entity<DailyEfficiency>()
                .HasIndex(e => new { e.Date, e.EmployeeId })
                .IsUnique()
                .HasDatabaseName("IX_DailyEfficiency_Date_Employee");


            modelBuilder.Entity<Shift>()
                .HasMany(s => s.Entries)
                .WithOne(e => e.Shift)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Department>()
               .HasMany(d => d.Employees)
               .WithOne(e => e.Department)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Routing>()
                .HasMany(r => r.Entries)
                .WithOne(e => e.Routing)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Entries)
                .WithOne(e => e.Employee)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Employees)
                .WithOne(e => e.ApplicationUser)
                .HasForeignKey(e => e.ApplicationUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Claims)
                .WithOne()
                .HasForeignKey(c => c.UserId)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Roles)
                .WithOne()
                .HasForeignKey(r => r.UserId)
                .IsRequired();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(u => u.Logins)
                .WithOne()
                .HasForeignKey(l => l.UserId)
                .IsRequired();

        }
    }
}
