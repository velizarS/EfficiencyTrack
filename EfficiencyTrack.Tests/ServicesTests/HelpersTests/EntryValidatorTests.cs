using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfficiencyTrack.Tests.ServicesTests.HelpersTest
{
    public class EntryValidatorTests
    {
        private DbContextOptions<EfficiencyTrackDbContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<EfficiencyTrackDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private Shift CreateShift(Guid? id = null, int durationMinutes = 480)
        {
            return new Shift
            {
                Id = id ?? Guid.NewGuid(),
                DurationMinutes = durationMinutes,
                Name = "Смяна 1"
            };
        }

        private Routing CreateRouting(Guid? id = null, decimal minutesPerPiece = 1m)
        {
            return new Routing
            {
                Id = id ?? Guid.NewGuid(),
                MinutesPerPiece = minutesPerPiece,
                Code = "2cds",
                Description = "Тестово описание",  
                Zone = "Зона A"
            };
        }

        [Fact]
        public async Task ValidateAsync_DuplicateEntry_ReturnsError()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new EfficiencyTrackDbContext(options, null);

            var employeeId = Guid.NewGuid();
            var routingId = Guid.NewGuid();

            var existingEntry = new Entry
            {
                EmployeeId = employeeId,
                RoutingId = routingId,
                Date = DateTime.UtcNow.Date,
                Pieces = 10,
                WorkedMinutes = 100,
                IsDeleted = false
            };
            await context.Entries.AddAsync(existingEntry);
            await context.SaveChangesAsync();

            var validator = new EntryValidator(context);

            var newEntry = new Entry
            {
                EmployeeId = employeeId,
                RoutingId = routingId,
                Date = DateTime.UtcNow.Date,
                Pieces = 10,
                WorkedMinutes = 100,
                IsDeleted = false
            };

            // Act
            var result = await validator.ValidateAsync(newEntry);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Вече сте добавили тези данни"));
        }

        [Fact]
        public async Task ValidateAsync_InvalidShift_ReturnsError()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new EfficiencyTrackDbContext(options, null);

            var validator = new EntryValidator(context);

            var entry = new Entry
            {
                ShiftId = Guid.NewGuid(), // несъществуващ
                EmployeeId = Guid.NewGuid(),
                Date = DateTime.UtcNow.Date,
                WorkedMinutes = 60,
                Pieces = 10,
                RoutingId = Guid.NewGuid()
            };

            // Act
            var result = await validator.ValidateAsync(entry);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Невалидна смяна"));
        }

        [Fact]
        public async Task ValidateAsync_ExceededShiftTime_ReturnsError()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new EfficiencyTrackDbContext(options, null);

            var shift = CreateShift(durationMinutes: 120);
            await context.Shifts.AddAsync(shift);
            await context.SaveChangesAsync();

            var employeeId = Guid.NewGuid();

            var existingEntry = new Entry
            {
                EmployeeId = employeeId,
                Date = DateTime.UtcNow.Date,
                WorkedMinutes = 100,
                IsDeleted = false
            };
            await context.Entries.AddAsync(existingEntry);
            await context.SaveChangesAsync();

            var validator = new EntryValidator(context);

            var newEntry = new Entry
            {
                EmployeeId = employeeId,
                Date = DateTime.UtcNow.Date,
                WorkedMinutes = 30, 
                ShiftId = shift.Id,
                Pieces = 10,
                RoutingId = Guid.NewGuid()
            };

            // Act
            var result = await validator.ValidateAsync(newEntry);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Превишено време") || e.Contains("изчерпали всички"));
        }

        [Fact]
        public async Task ValidateAsync_InvalidRouting_ReturnsError()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new EfficiencyTrackDbContext(options, null);

            var shift = CreateShift();
            await context.Shifts.AddAsync(shift);
            await context.SaveChangesAsync();

            var validator = new EntryValidator(context);

            var entry = new Entry
            {
                ShiftId = shift.Id,
                EmployeeId = Guid.NewGuid(),
                Date = DateTime.UtcNow.Date,
                RoutingId = Guid.NewGuid(), 
                WorkedMinutes = 60,
                Pieces = 10,
            };

            // Act
            var result = await validator.ValidateAsync(entry);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("Невалиден RoutingId"));
        }

        [Fact]
        public async Task ValidateAsync_EfficiencyTooHigh_ReturnsError()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new EfficiencyTrackDbContext(options, null);

            var shift = CreateShift();
            var routing = CreateRouting(minutesPerPiece: 1m);
            await context.Shifts.AddAsync(shift);
            await context.Routings.AddAsync(routing);
            await context.SaveChangesAsync();

            var validator = new EntryValidator(context);

            var entry = new Entry
            {
                ShiftId = shift.Id,
                EmployeeId = Guid.NewGuid(),
                Date = DateTime.UtcNow.Date,
                RoutingId = routing.Id,
                WorkedMinutes = 10,
                Pieces = 10,
                Scrap = 10 
            };

            // Act
            var result = await validator.ValidateAsync(entry);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.Contains("прекалено висока"));
        }

        [Fact]
        public async Task ValidateAsync_WorkedMinutesOrPiecesZero_ReturnsError()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new EfficiencyTrackDbContext(options, null);

            var shift = CreateShift();
            var routing = CreateRouting();
            await context.Shifts.AddAsync(shift);
            await context.Routings.AddAsync(routing);
            await context.SaveChangesAsync();

            var validator = new EntryValidator(context);

            var entry1 = new Entry
            {
                ShiftId = shift.Id,
                EmployeeId = Guid.NewGuid(),
                Date = DateTime.UtcNow.Date,
                RoutingId = routing.Id,
                WorkedMinutes = 0,
                Pieces = 10,
            };

            var entry2 = new Entry
            {
                ShiftId = shift.Id,
                EmployeeId = Guid.NewGuid(),
                Date = DateTime.UtcNow.Date,
                RoutingId = routing.Id,
                WorkedMinutes = 10,
                Pieces = 0,
                Scrap = 0,
            };

            // Act
            var result1 = await validator.ValidateAsync(entry1);
            var result2 = await validator.ValidateAsync(entry2);

            // Assert
            Assert.False(result1.IsValid);
            Assert.Contains(result1.Errors, e => e.Contains("грешка"));

            Assert.False(result2.IsValid);
            Assert.Contains(result2.Errors, e => e.Contains("грешка"));
        }

        [Fact]
        public async Task ValidateAsync_ValidEntry_ReturnsValid()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new EfficiencyTrackDbContext(options, null);

            var shift = CreateShift();
            var routing = CreateRouting();
            await context.Shifts.AddAsync(shift);
            await context.Routings.AddAsync(routing);
            await context.SaveChangesAsync();

            var validator = new EntryValidator(context);

            var entry = new Entry
            {
                ShiftId = shift.Id,
                EmployeeId = Guid.NewGuid(),
                Date = DateTime.UtcNow.Date,
                RoutingId = routing.Id,
                WorkedMinutes = 60,
                Pieces = 60,
                Scrap = 0
            };

            // Act
            var result = await validator.ValidateAsync(entry);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }
    }
}
