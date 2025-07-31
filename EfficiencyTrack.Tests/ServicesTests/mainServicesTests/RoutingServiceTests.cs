using System;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Principal;
using System.Security.Claims;
using Xunit;

namespace EfficiencyTrack.Tests.Services
{
    public class RoutingServiceTests
    {
        private readonly EfficiencyTrackDbContext _context;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly RoutingService _service;

        public RoutingServiceTests()
        {
            var options = new DbContextOptionsBuilder<EfficiencyTrackDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var identityMock = new Mock<IIdentity>();
            identityMock.Setup(i => i.Name).Returns("TestUser");
            identityMock.Setup(i => i.IsAuthenticated).Returns(true);

            var userMock = new Mock<ClaimsPrincipal>();
            userMock.Setup(u => u.Identity).Returns(identityMock.Object);

            var httpContextMock = new Mock<HttpContext>();
            httpContextMock.Setup(c => c.User).Returns(userMock.Object);

            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(httpContextMock.Object);

            _context = new EfficiencyTrackDbContext(options, _httpContextAccessorMock.Object);

            _service = new RoutingService(_context, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task AddAsync_ValidRouting_AddsRoutingSuccessfully()
        {
            // Arrange
            var routing = new Routing
            {
                Id = Guid.NewGuid(),
                Code = "R001",
                Zone = "Zone1",
                MinutesPerPiece = 10,
                DepartmentId = Guid.NewGuid(),
                Description = "Test routing"
            };

            // Act
            var result = await _service.AddAsync(routing);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("R001", result.Code);

            var dbEntry = await _context.Routings.FirstOrDefaultAsync(r => r.Code == "R001");
            Assert.NotNull(dbEntry);
            Assert.Equal("TestUser", dbEntry.CreatedBy);
            Assert.False(dbEntry.IsDeleted);
        }

        [Fact]
        public async Task AddAsync_DuplicateCode_ThrowsInvalidOperationException()
        {
            // Arrange
            var routing1 = new Routing
            {
                Id = Guid.NewGuid(),
                Code = "R002",
                Zone = "Zone1",
                MinutesPerPiece = 15,
                DepartmentId = Guid.NewGuid(),
                Description = "First routing"
            };
            await _service.AddAsync(routing1);

            var routing2 = new Routing
            {
                Id = Guid.NewGuid(),
                Code = "R002",
                Zone = "Zone2",
                MinutesPerPiece = 20,
                DepartmentId = Guid.NewGuid(),
                Description = "Duplicate routing"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddAsync(routing2));
        }
    }
}
