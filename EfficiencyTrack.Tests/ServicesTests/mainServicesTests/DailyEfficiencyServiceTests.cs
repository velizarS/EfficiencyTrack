using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EfficiencyTrack.Tests.ServicesTests.mainServicesTests
{
    public class DailyEfficiencyServiceTests
    {
        private EfficiencyTrackDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EfficiencyTrackDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

            return new EfficiencyTrackDbContext(options, mockHttpContextAccessor.Object);
        }

        private DailyEfficiencyService GetService(EfficiencyTrackDbContext context)
            => new DailyEfficiencyService(context);

        [Fact]
        public async Task GetAllAsync_ReturnsAllWithIncludesAndOrdered()
        {
            var context = GetInMemoryDbContext();

            var emp = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Code = "EMP001"
            };
            var shift = new Shift
            {
                Id = Guid.NewGuid(),
                Name = "Morning"
            };

            context.Employees.Add(emp);
            context.Shifts.Add(shift);

            context.DailyEfficiencies.AddRange(
                new DailyEfficiency
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.UtcNow.AddDays(-1),
                    Employee = emp,
                    Shift = shift,
                    IsDeleted = false
                },
                new DailyEfficiency
                {
                    Id = Guid.NewGuid(),
                    Date = DateTime.UtcNow,
                    Employee = emp,
                    Shift = shift,
                    IsDeleted = false
                });

            await context.SaveChangesAsync();

            var service = GetService(context);
            var all = await service.GetAllAsync();

            Assert.Equal(2, all.Count());
            Assert.True(all.First().Date > all.Last().Date);
            Assert.All(all, de => Assert.NotNull(de.Employee));
            Assert.All(all, de => Assert.NotNull(de.Shift));
        }

        [Fact]
        public async Task GetByShiftManagerIdAsync_FiltersByShiftManagerUserId()
        {
            var context = GetInMemoryDbContext();

            var shiftManagerId = Guid.NewGuid();

            var emp1 = new Employee
            {
                Id = Guid.NewGuid(),
                Code = "E001",
                FirstName = "Anna",
                LastName = "Ivanova",
                MiddleName = "M.",
                ShiftManagerUserId = shiftManagerId
            };

            var emp2 = new Employee
            {
                Id = Guid.NewGuid(),
                Code = "E002",
                FirstName = "Georgi",
                LastName = "Petrov",
                MiddleName = "P.",
                ShiftManagerUserId = Guid.NewGuid()
            };

            context.Employees.AddRange(emp1, emp2);

            var shift = new Shift
            {
                Id = Guid.NewGuid(),
                Name = "Night Shift"
            };

            context.Shifts.Add(shift);

            context.DailyEfficiencies.AddRange(
                new DailyEfficiency
                {
                    Id = Guid.NewGuid(),
                    Employee = emp1,
                    EmployeeId = emp1.Id,
                    Shift = shift,
                    ShiftId = shift.Id,
                    IsDeleted = false,
                    Date = DateTime.Today
                },
                new DailyEfficiency
                {
                    Id = Guid.NewGuid(),
                    Employee = emp2,
                    EmployeeId = emp2.Id,
                    Shift = shift,
                    ShiftId = shift.Id,
                    IsDeleted = false,
                    Date = DateTime.Today
                }
            );

            await context.SaveChangesAsync();

            var service = GetService(context);
            var result = await service.GetByShiftManagerIdAsync(shiftManagerId);

            Assert.Single(result);
            Assert.Equal(emp1.Id, result.First().EmployeeId);
        }


        [Fact]
        public async Task GetByIdAsync_InvalidId_Throws()
        {
            var context = GetInMemoryDbContext();
            var service = GetService(context);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await service.GetByIdAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            var context = GetInMemoryDbContext();
            var service = GetService(context);

            var dto = await service.GetByIdAsync(Guid.NewGuid());
            Assert.Null(dto);
        }

        [Fact]
        public async Task GetByIdAsync_Existing_ReturnsDtoWithEntries()
        {
            var context = GetInMemoryDbContext();

            var testDate = new DateTime(2025, 7, 31);

            var emp = new Employee
            {
                Id = Guid.NewGuid(),
                Code = "E123",
                FirstName = "Ivan",
                MiddleName = "M.",
                LastName = "Petrov"
            };

            var shift = new Shift
            {
                Id = Guid.NewGuid(),
                Name = "Shift A"
            };

            var routing = new Routing
            {
                Id = Guid.NewGuid(),
                Code = "R001",
                Description = "Test Operation",
                Zone = "Z1",
                MinutesPerPiece = 1,
                IsDeleted = false
            };

            context.Employees.Add(emp);
            context.Shifts.Add(shift);
            context.Routings.Add(routing);

            var dailyEfficiency = new DailyEfficiency
            {
                Id = Guid.NewGuid(),
                Employee = emp,
                EmployeeId = emp.Id,
                Shift = shift,
                ShiftId = shift.Id,
                Date = testDate,
                IsDeleted = false,
                EfficiencyPercentage = 90,
                TotalNeededMinutes = 100,
                TotalWorkedMinutes = 95
            };

            context.DailyEfficiencies.Add(dailyEfficiency);

            var entry = new Entry
            {
                Id = Guid.NewGuid(),
                EmployeeId = emp.Id,
                ShiftId = shift.Id,
                Date = testDate,
                RoutingId = routing.Id,
                Routing = routing,
                Pieces = 10,
                WorkedMinutes = 50,
                EfficiencyForOperation = 80,
                Scrap = 0,
                IsDeleted = false
            };

            context.Entries.Add(entry);

            await context.SaveChangesAsync();

            var service = GetService(context);
            var dto = await service.GetByIdAsync(dailyEfficiency.Id);

            Assert.NotNull(dto);
            Assert.Equal(emp.Code, dto.EmployeeCode);
            Assert.Contains("Ivan", dto.EmployeeFullName);
            Assert.Equal(90, dto.EfficiencyPercentage);
            Assert.Single(dto.Entries);

            var dtoEntry = dto.Entries[0];
            Assert.Equal(entry.Pieces, dtoEntry.Pieces);
            Assert.Equal(routing.Code, dtoEntry.RoutingName);
            Assert.Equal(entry.WorkedMinutes, dtoEntry.WorkedMinutes);
            Assert.Equal(entry.EfficiencyForOperation, dtoEntry.EfficiencyForOperation);
        }



        [Fact]
        public async Task UpdateDailyEfficiencyAsync_NoEntries_RemovesExisting()
        {
            var context = GetInMemoryDbContext();

            var empId = Guid.NewGuid();
            var shiftId = Guid.NewGuid();
            var testDate = new DateTime(2025, 7, 31);

            var employee = new Employee
            {
                Id = empId,
                Code = "EMP001",
                FirstName = "Ivan",
                LastName = "Petrov"
            };

            var shift = new Shift
            {
                Id = shiftId,
                Name = "Shift A",
                DurationMinutes = 480
            };

            context.Employees.Add(employee);
            context.Shifts.Add(shift);

            var dailyEfficiency = new DailyEfficiency
            {
                Id = Guid.NewGuid(),
                EmployeeId = empId,
                ShiftId = shiftId,
                Date = testDate,
                EfficiencyPercentage = 50,
                TotalNeededMinutes = 100,
                TotalWorkedMinutes = 90
            };
            context.DailyEfficiencies.Add(dailyEfficiency);

            await context.SaveChangesAsync();

            var service = GetService(context);
            await service.UpdateDailyEfficiencyAsync(empId, testDate);

            using var verificationContext = GetInMemoryDbContext();
            var stillExists = await verificationContext.DailyEfficiencies.AnyAsync(de => de.Id == dailyEfficiency.Id);

            Assert.False(stillExists);
        }



        [Fact]
        public async Task UpdateDailyEfficiencyAsync_EntriesWithMultipleShifts_Throws()
        {
            var context = GetInMemoryDbContext();

            var empId = Guid.NewGuid();
            var testDate = new DateTime(2025, 7, 31);

            var shift1 = new Shift { Id = Guid.NewGuid(), Name = "Shift 1", DurationMinutes = 480 };
            var shift2 = new Shift { Id = Guid.NewGuid(), Name = "Shift 2", DurationMinutes = 480 };
            context.Shifts.AddRange(shift1, shift2);

            var routing = new Routing
            {
                Id = Guid.NewGuid(),
                Code = "R001",
                Description = "Test routing",
                Zone = "A",
                MinutesPerPiece = 1,
                IsDeleted = false
            };
            context.Routings.Add(routing);

            context.Entries.AddRange(
                new Entry
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = empId,
                    ShiftId = shift1.Id,
                    Date = testDate,
                    Pieces = 1,
                    WorkedMinutes = 60,
                    RoutingId = routing.Id,
                    Routing = routing,
                    EfficiencyForOperation = 90,
                    Scrap = 0,
                    IsDeleted = false
                },
                new Entry
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = empId,
                    ShiftId = shift2.Id,
                    Date = testDate,
                    Pieces = 1,
                    WorkedMinutes = 60,
                    RoutingId = routing.Id,
                    Routing = routing,
                    EfficiencyForOperation = 85,
                    Scrap = 0,
                    IsDeleted = false
                }
            );

            await context.SaveChangesAsync();

            var service = GetService(context);

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.UpdateDailyEfficiencyAsync(empId, testDate));
        }


        [Fact]
        public async Task UpdateDailyEfficiencyAsync_ValidEntries_AddsOrUpdatesDailyEfficiency()
        {
            var context = GetInMemoryDbContext();

            var empId = Guid.NewGuid();
            var shiftId = Guid.NewGuid();
            var routingId = Guid.NewGuid();

            var testDate = new DateTime(2025, 7, 31);

            var shift = new Shift { Id = shiftId, Name = "Shift A", DurationMinutes = 480 };
            var routing = new Routing
            {
                Id = routingId,
                Code = "R001",
                Description = "Test routing",
                Zone = "Zone A",
                MinutesPerPiece = 2,
                IsDeleted = false
            };
            context.Shifts.Add(shift);
            context.Routings.Add(routing);

            context.Entries.Add(new Entry
            {
                Id = Guid.NewGuid(),
                EmployeeId = empId,
                ShiftId = shiftId,
                Date = testDate,
                Pieces = 10,
                Scrap = 2,
                WorkedMinutes = 60,
                RoutingId = routingId,
                IsDeleted = false
            });

            await context.SaveChangesAsync();

            var service = GetService(context);

            await service.UpdateDailyEfficiencyAsync(empId, testDate);
            var dailyEff = await context.DailyEfficiencies.FirstOrDefaultAsync(de => de.EmployeeId == empId && de.Date == testDate);
            Assert.NotNull(dailyEff);
            Assert.Equal(24, dailyEff.TotalNeededMinutes);
            Assert.Equal(60, dailyEff.TotalWorkedMinutes);

            dailyEff.TotalWorkedMinutes = 30;
            await context.SaveChangesAsync();

            await service.UpdateDailyEfficiencyAsync(empId, testDate);

            var updatedDailyEff = await context.DailyEfficiencies.FirstOrDefaultAsync(de => de.EmployeeId == empId && de.Date == testDate);
            Assert.NotNull(updatedDailyEff);
            Assert.Equal(60, updatedDailyEff.TotalWorkedMinutes);
        }

    }
}
