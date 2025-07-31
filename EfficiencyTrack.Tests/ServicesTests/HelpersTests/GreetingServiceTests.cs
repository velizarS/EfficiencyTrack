using System;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Helpers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfficiencyTrack.Tests.Services.Helpers
{
    public class GreetingServiceTests
    {
        private DbContextOptions<EfficiencyTrackDbContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<EfficiencyTrackDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private Employee CreateValidEmployee(Guid? id = null, string firstName = "Иван")
        {
            return new Employee
            {
                Id = id ?? Guid.NewGuid(),
                FirstName = firstName,
                LastName = "Тестов",
                Code = "EMP123",
                DepartmentId = Guid.NewGuid(),
            };
        }

        [Fact]
        public async Task GetGreetingMessageAsync_FirstEntryToday_ReturnsGreetingWithName()
        {
            // Arrange
            var options = CreateInMemoryOptions();

            using var context = new EfficiencyTrackDbContext(options, null);
            var employee = CreateValidEmployee(firstName: "Иван");

            await context.Employees.AddAsync(employee);
            await context.SaveChangesAsync();

            var entry = new Entry
            {
                EmployeeId = employee.Id,
                EfficiencyForOperation = 95,
                Date = DateTime.UtcNow.Date
            };

            await context.Entries.AddAsync(entry);
            await context.SaveChangesAsync();

            var service = new GreetingService(context);

            // Act
            var result = await service.GetGreetingMessageAsync(entry);

            // Assert
            Assert.Contains("Здравейте, Иван!", result);
        }

        [Fact]
        public async Task GetGreetingMessageAsync_SecondEntryToday_DoesNotIncludeGreeting()
        {
            // Arrange
            var options = CreateInMemoryOptions();

            using var context = new EfficiencyTrackDbContext(options, null);
            var employeeId = Guid.NewGuid();
            var employee = CreateValidEmployee(employeeId, "Мария");

            await context.Employees.AddAsync(employee);

            var firstEntry = new Entry
            {
                EmployeeId = employeeId,
                Date = DateTime.UtcNow.Date
            };

            await context.Entries.AddAsync(firstEntry);
            await context.SaveChangesAsync();

            var secondEntry = new Entry
            {
                EmployeeId = employeeId,
                EfficiencyForOperation = 91,
                Date = DateTime.UtcNow.Date
            };

            await context.Entries.AddAsync(secondEntry);
            await context.SaveChangesAsync();

            var service = new GreetingService(context);

            // Act
            var result = await service.GetGreetingMessageAsync(secondEntry);

            // Assert
            Assert.DoesNotContain("Здравейте, Мария!", result);
            Assert.Contains("Добра работа!", result);
        }

        [Theory]
        [InlineData(101, "надминава очакванията ни")]
        [InlineData(90, "оправдава очакванията ни")]
        [InlineData(85, "необходимо е подобрение")]
        [InlineData(60, "Незадоволително представяне")]
        public async Task GetGreetingMessageAsync_EfficiencyRanges_ReturnsCorrectMessage(double efficiency, string expected)
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new EfficiencyTrackDbContext(options, null);

            var employeeId = Guid.NewGuid();
            var employee = CreateValidEmployee(employeeId);
            await context.Employees.AddAsync(employee);
            await context.SaveChangesAsync();

            var entry = new Entry
            {
                EmployeeId = employeeId,
                EfficiencyForOperation = (decimal)efficiency,
                Date = DateTime.UtcNow.Date
            };

            await context.Entries.AddAsync(entry);
            await context.SaveChangesAsync();

            var service = new GreetingService(context);

            // Act
            var result = await service.GetGreetingMessageAsync(entry);

            // Assert
            Assert.Contains(expected, result);
        }

        [Fact]
        public async Task GetGreetingMessageAsync_NoEmployeeName_DoesNotThrow()
        {
            // Arrange
            var options = CreateInMemoryOptions();
            using var context = new EfficiencyTrackDbContext(options, null);

            var employeeId = Guid.NewGuid();
            var employee = new Employee
            {
                Id = employeeId,
                FirstName = string.Empty,
                LastName = "БезИме",
                Code = "EMP000",
                DepartmentId = Guid.NewGuid(),
            };

            await context.Employees.AddAsync(employee);
            await context.SaveChangesAsync();

            var entry = new Entry
            {
                EmployeeId = employeeId,
                EfficiencyForOperation = 95,
                Date = DateTime.UtcNow.Date
            };

            await context.Entries.AddAsync(entry);
            await context.SaveChangesAsync();

            var service = new GreetingService(context);

            // Act
            var result = await service.GetGreetingMessageAsync(entry);

            // Assert
            Assert.DoesNotContain("Здравейте", result);
        }

        [Fact]
        public void Constructor_NullContext_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new GreetingService(null));
        }
    }
}
