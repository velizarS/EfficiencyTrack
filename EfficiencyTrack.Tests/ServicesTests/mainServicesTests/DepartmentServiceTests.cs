using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using EfficiencyTrack.Services.Helpers;

namespace EfficiencyTrack.Tests.ServicesTests.mainServicesTests
{
    public class DepartmentServiceTests
    {
        private static EfficiencyTrackDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<EfficiencyTrackDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new EfficiencyTrackDbContext(options, new Mock<IHttpContextAccessor>().Object);
        }

        private static DepartmentService CreateService(EfficiencyTrackDbContext context)
        {
            var accessor = new Mock<IHttpContextAccessor>();
            return new DepartmentService(context, accessor.Object);
        }

        private static Department CreateDepartment(string name = "TestDept")
        {
            return new Department
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        }

        [Fact]
        public async Task AddAsync_AddsDepartmentSuccessfully()
        {
            using var context = CreateDbContext();
            var service = CreateService(context);
            var dept = CreateDepartment("Dev");

            var result = await service.AddAsync(dept);

            Assert.NotNull(result);
            Assert.Equal("Dev", result.Name);
        }

        [Fact]
        public async Task AddAsync_DuplicateName_ThrowsException()
        {
            using var context = CreateDbContext();
            var service = CreateService(context);

            await service.AddAsync(CreateDepartment("Dev"));

            await Assert.ThrowsAsync<DuplicateDepartmentException>(() =>
                service.AddAsync(CreateDepartment("Dev")));
        }

        [Fact]
        public async Task UpdateAsync_ValidUpdate_Succeeds()
        {
            using var context = CreateDbContext();
            var service = CreateService(context);

            var dept = await service.AddAsync(CreateDepartment("OldName"));
            dept.Name = "NewName";

            var updated = await service.UpdateAsync(dept);
            Assert.True(updated);
        }

        [Fact]
        public async Task UpdateAsync_DuplicateName_ThrowsException()
        {
            using var context = CreateDbContext();
            var service = CreateService(context);

            var dept1 = await service.AddAsync(CreateDepartment("Dept1"));
            var dept2 = await service.AddAsync(CreateDepartment("Dept2"));

            dept2.Name = "Dept1";

            await Assert.ThrowsAsync<DuplicateDepartmentException>(() => service.UpdateAsync(dept2));
        }

        [Fact]
        public async Task GetDepartmentWithEmployeesAsync_ReturnsDepartmentWithEmployees()
        {
            using var context = CreateDbContext();
            var service = CreateService(context);

            var depId = Guid.NewGuid();
            var department = new Department
            {
                Id = depId,
                Name = "QA",
                Employees = new List<Employee>
{
    new Employee
    {
        Id = Guid.NewGuid(),
        FirstName = "John",
        LastName = "Doe",
        Code = "E1",
        DepartmentId = depId
    }
}

            };

            context.Departments.Add(department);
            await context.SaveChangesAsync();

            var result = await service.GetDepartmentWithEmployeesAsync(depId);

            Assert.NotNull(result);
            Assert.Equal("QA", result!.Name);
            Assert.Single(result.Employees);
        }
    }
}
