using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EfficiencyTrack.Tests.ServicesTests.mainServicesTests
{
    public class EmployeeServiceTests
    {
        private EfficiencyTrackDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<EfficiencyTrackDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new EfficiencyTrackDbContext(options, new Mock<IHttpContextAccessor>().Object);
        }

        private Employee CreateEmployee(string code = "EMP001", Guid? id = null)
        {
            return new Employee
            {
                Id = id ?? Guid.NewGuid(),
                FirstName = "Тест",
                LastName = "Потребител",
                Code = code,
                DepartmentId = Guid.NewGuid()
            };
        }


        private EmployeeService CreateService(EfficiencyTrackDbContext context)
        {
            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            return new EmployeeService(context, new Mock<IHttpContextAccessor>().Object, userManagerMock.Object);
        }

        [Fact]
        public async Task AddAsync_AddsEmployeeSuccessfully()
        {
            // Arrange
            using var context = CreateDbContext();

            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = "Test Department",
            };
            await context.Departments.AddAsync(department);
            await context.SaveChangesAsync();

            var service = CreateService(context);
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Иван",
                LastName = "Тестов",
                Code = Guid.NewGuid().ToString(), 
                DepartmentId = department.Id
            };

            // Act
            await service.AddAsync(employee);
            var all = await service.GetAllAsync();

            // Assert
            Assert.Single(all);
            Assert.Equal(employee.Code, all.First().Code);
        }


        [Fact]
        public async Task UpdateAsync_UpdatesEmployee()
        {
            using var context = CreateDbContext();
            var service = CreateService(context);

            // Подготви и добави служителя
            var department = new Department { Id = Guid.NewGuid(), Name = "D1" };
            await context.Departments.AddAsync(department);
            await context.SaveChangesAsync();

            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Иван",
                LastName = "Тестов",
                Code = "EMP001",
                DepartmentId = department.Id,
            };

            await service.AddAsync(employee);

            // Промени и обнови
            employee.FirstName = "Обновено";
            await service.UpdateAsync(employee);

            // Извлечи отново
            var updated = await service.GetByIdAsync(employee.Id);

            Assert.NotNull(updated);
            Assert.Equal("Обновено", updated?.FirstName);
        }


        [Fact]
        public async Task GetByCodeAsync_ReturnsCorrectEmployee()
        {
            // Arrange
            using var context = CreateDbContext();

            // Добави валиден Department
            var department = new Department
            {
                Id = Guid.NewGuid(),
                Name = "TestDept"
            };
            await context.Departments.AddAsync(department);
            await context.SaveChangesAsync();

            // Уникален код
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FirstName = "Иван",
                LastName = "Петров",
                Code = "X123",
                DepartmentId = department.Id,
            };

            var service = CreateService(context);

            // Act
            await service.AddAsync(employee);
            var result = await service.GetByCodeAsync("X123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(employee.Id, result?.Id);
        }


        [Fact]
        public async Task IsEmployeeCodeUniqueAsync_ReturnsFalse_IfExists()
        {
            using var context = CreateDbContext();
            var service = CreateService(context);
            var emp = CreateEmployee("DUPLICATE");
            await service.AddAsync(emp);

            var isUnique = await service.IsEmployeeCodeUniqueAsync("DUPLICATE");
            Assert.False(isUnique);
        }

        [Fact]
        public async Task GetByDepartmentAsync_ReturnsCorrectEmployees()
        {
            using var context = CreateDbContext();
            var service = CreateService(context);
            var depId = Guid.NewGuid();

            var employeeA = CreateEmployee("A", id: Guid.NewGuid());
            employeeA.DepartmentId = depId;
            await service.AddAsync(employeeA);

            var employeeB = CreateEmployee("B", id: Guid.NewGuid());
            await service.AddAsync(employeeB);

            var result = await service.GetByDepartmentAsync(depId);

            Assert.Single(result);
            Assert.Equal(depId, result.First().DepartmentId);
        }


        [Fact]
        public async Task GetByShiftManagerUserIdAsync_ReturnsCorrectEmployees()
        {
            using var context = CreateDbContext();
            var service = CreateService(context);
            var shiftManagerId = Guid.NewGuid();

            var employeeA = CreateEmployee("A", id: Guid.NewGuid());
            employeeA.ShiftManagerUserId = shiftManagerId;
            await service.AddAsync(employeeA);

            var employeeB = CreateEmployee("B", id: Guid.NewGuid());
            await service.AddAsync(employeeB);

            var result = await service.GetByShiftManagerUserIdAsync(shiftManagerId);

            Assert.Single(result);
            Assert.Equal(shiftManagerId, result.First().ShiftManagerUserId);
        }


        [Fact]
        public async Task GetAllShiftManagersAsync_ReturnsUsersInRole()
        {
            var context = CreateDbContext();

            var userList = new List<ApplicationUser>
            {
                new() { UserName = "manager1" },
                new() { UserName = "manager2" }
            };

            var userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            userManagerMock.Setup(x => x.GetUsersInRoleAsync("ShiftLeader"))
                           .ReturnsAsync(userList);

            var service = new EmployeeService(context, new Mock<IHttpContextAccessor>().Object, userManagerMock.Object);
            var result = await service.GetAllShiftManagersAsync();

            Assert.Equal(2, result.Count);
        }
    }
}
