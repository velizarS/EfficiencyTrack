using Xunit;
using Moq;
using Microsoft.EntityFrameworkCore;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Implementations;
using EfficiencyTrack.Services.Interfaces;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace EfficiencyTrack.Tests.ServicesTests.mainServicesTests
{
    public class EntryServiceTests
    {
        private EfficiencyTrackDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EfficiencyTrackDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            
            var context = new EfficiencyTrackDbContext(options, httpContextAccessorMock.Object);
            return context;
        }


        private EntryService GetService(EfficiencyTrackDbContext context, Mock<IDailyEfficiencyService>? dailyEfficiencyMock = null, Mock<IGreetingService>? greetingMock = null)
        {
            var dailyEfficiencyService = dailyEfficiencyMock?.Object ?? Mock.Of<IDailyEfficiencyService>();
            var greetingService = greetingMock?.Object ?? Mock.Of<IGreetingService>();

            var httpContextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();

            return new EntryService(context, httpContextAccessorMock.Object, dailyEfficiencyService, greetingService);
        }

        private async Task SeedData(EfficiencyTrackDbContext context)
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                Code = "EMP001",       
                LastName = "Ivanov",   
                FirstName = "Ivan",
                ShiftManagerUserId = Guid.NewGuid(),
                DepartmentId = Guid.NewGuid(),
                MiddleName = "Test",
                
            };

            context.Employees.Add(employee);

            var shift = new Shift
            {
                Id = Guid.NewGuid(),
                Name = "Morning Shift",
                DurationMinutes = 300,
            };
            context.Shifts.Add(shift);

            var routing = new Routing
            {
                Id = Guid.NewGuid(),
                Code = "Routing 1",
                MinutesPerPiece = 2.5m,
                DepartmentId = Guid.NewGuid(),
                Description = "Description",
                Zone = "zone"
            };
            context.Routings.Add(routing);

            await context.SaveChangesAsync();
        }


        [Fact]
        public async Task AddAsync_ValidEntry_AddsEntryAndUpdatesDailyEfficiency()
        {
            var context = GetInMemoryDbContext();
            await SeedData(context);

            var dailyEfficiencyMock = new Mock<IDailyEfficiencyService>();
            dailyEfficiencyMock.Setup(s => s.UpdateDailyEfficiencyAsync(It.IsAny<Guid>(), It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            var service = GetService(context, dailyEfficiencyMock);

            var employee = context.Employees.First();
            var routing = context.Routings.First();

            var newEntry = new Entry
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                ShiftId = context.Shifts.First().Id,
                Date = DateTime.Today,
                Pieces = 10,
                Scrap = 2,
                WorkedMinutes = 24,
                RoutingId = routing.Id,
                IsDeleted = false,
            };

            var addedEntry = await service.AddAsync(newEntry);

            Assert.NotNull(addedEntry);
            Assert.Equal(newEntry.EmployeeId, addedEntry.EmployeeId);
            Assert.True(addedEntry.RequiredMinutes > 0);
            Assert.True(addedEntry.EfficiencyForOperation > 0);

            dailyEfficiencyMock.Verify(s => s.UpdateDailyEfficiencyAsync(newEntry.EmployeeId, newEntry.Date), Times.Once);

            var entryInDb = await context.Entries.FindAsync(newEntry.Id);
            Assert.NotNull(entryInDb);
        }

        [Fact]
        public async Task UpdateAsync_ValidEntry_UpdatesEntryAndUpdatesDailyEfficiency()
        {
            var context = GetInMemoryDbContext();
            await SeedData(context);

            var dailyEfficiencyMock = new Mock<IDailyEfficiencyService>();
            dailyEfficiencyMock.Setup(s => s.UpdateDailyEfficiencyAsync(It.IsAny<Guid>(), It.IsAny<DateTime>())).Returns(Task.CompletedTask);

            var service = GetService(context, dailyEfficiencyMock);

            var employee = context.Employees.First();
            var routing = context.Routings.First();

            var entry = new Entry
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                ShiftId = context.Shifts.First().Id,
                Date = DateTime.Today,
                Pieces = 5,
                Scrap = 1,
                WorkedMinutes = 20,
                RoutingId = routing.Id,
                IsDeleted = false
            };

            await context.Entries.AddAsync(entry);
            await context.SaveChangesAsync();

            entry.Pieces = 8;
            entry.WorkedMinutes = 18;

            var result = await service.UpdateAsync(entry);

            Assert.True(result);

            var updatedEntry = await context.Entries.FindAsync(entry.Id);
            Assert.NotNull(updatedEntry);
            Assert.Equal(8, updatedEntry.Pieces);
            Assert.True(updatedEntry.EfficiencyForOperation > 0);

            dailyEfficiencyMock.Verify(s => s.UpdateDailyEfficiencyAsync(entry.EmployeeId, entry.Date), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ExistingEntry_DeletesEntryAndUpdatesDailyEfficiency()
        {
            var context = GetInMemoryDbContext();
            await SeedData(context);

            var dailyEfficiencyMock = new Mock<IDailyEfficiencyService>();
            dailyEfficiencyMock
                .Setup(s => s.UpdateDailyEfficiencyAsync(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask);

            var service = GetService(context, dailyEfficiencyMock);

            var employee = context.Employees.First();
            var routing = context.Routings.First();

            var entry = new Entry
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                ShiftId = context.Shifts.First().Id,
                Date = DateTime.Today,
                Pieces = 5,
                Scrap = 1,
                WorkedMinutes = 20,
                RoutingId = routing.Id,
                IsDeleted = false
            };

            await context.Entries.AddAsync(entry);
            await context.SaveChangesAsync();

            var result = await service.DeleteAsync(entry.Id);

            Assert.True(result);

            var deletedEntry = await context.Entries.FindAsync(entry.Id);
            Assert.NotNull(deletedEntry);
            Assert.True(deletedEntry.IsDeleted);

            dailyEfficiencyMock.Verify(s => s.UpdateDailyEfficiencyAsync(entry.EmployeeId, entry.Date), Times.Once);
        }


        [Fact]
        public async Task GetAllWithIncludesAsync_ReturnsEntriesWithRelatedData()
        {
            var context = GetInMemoryDbContext();
            await SeedData(context);

            var service = GetService(context);

            var employee = context.Employees.First();
            var routing = context.Routings.First();

            var entry = new Entry
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                ShiftId = context.Shifts.First().Id,
                Date = DateTime.Today,
                Pieces = 5,
                Scrap = 1,
                WorkedMinutes = 20,
                RoutingId = routing.Id,
                IsDeleted = false
            };

            await context.Entries.AddAsync(entry);
            await context.SaveChangesAsync();

            var result = await service.GetAllWithIncludesAsync();

            Assert.NotEmpty(result);
            Assert.All(result, e =>
            {
                Assert.NotNull(e.Employee);
                Assert.NotNull(e.Routing);
            });
        }

        [Fact]
        public async Task GetByIdWithIncludesAsync_ReturnsEntryWithRelatedData()
        {
            var context = GetInMemoryDbContext();
            await SeedData(context);

            var service = GetService(context);

            var employee = context.Employees.First();
            var routing = context.Routings.First();

            var entry = new Entry
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                ShiftId = context.Shifts.First().Id,
                Date = DateTime.Today,
                Pieces = 5,
                Scrap = 1,
                WorkedMinutes = 20,
                RoutingId = routing.Id,
                IsDeleted = false
            };

            await context.Entries.AddAsync(entry);
            await context.SaveChangesAsync();

            var result = await service.GetByIdWithIncludesAsync(entry.Id);

            Assert.NotNull(result);
            Assert.Equal(entry.Id, result.Id);
            Assert.NotNull(result.Employee);
            Assert.NotNull(result.Routing);
        }

        [Fact]
        public async Task AddAsync_InvalidRouting_ThrowsInvalidOperationException()
        {
            var context = GetInMemoryDbContext();
            await SeedData(context);

            var dailyEfficiencyMock = new Mock<IDailyEfficiencyService>();

            var service = GetService(context, dailyEfficiencyMock);

            var employee = context.Employees.First();

            var invalidEntry = new Entry
            {
                Id = Guid.NewGuid(),
                EmployeeId = employee.Id,
                ShiftId = context.Shifts.First().Id,
                Date = DateTime.Today,
                Pieces = 5,
                Scrap = 1,
                WorkedMinutes = 20,
                RoutingId = Guid.NewGuid(), 
                IsDeleted = false
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddAsync(invalidEntry));
        }

        [Fact]
        public async Task Greetings_ReturnsGreetingMessage()
        {
            var context = GetInMemoryDbContext();

            var greetingMock = new Mock<IGreetingService>();
            greetingMock.Setup(s => s.GetGreetingMessageAsync(It.IsAny<Entry>()))
                .ReturnsAsync("Hello Test");

            var service = GetService(context, greetingMock: greetingMock);

            var entry = new Entry();

            var greeting = await service.Greetings(entry);

            Assert.Equal("Hello Test", greeting);
        }
    }
}