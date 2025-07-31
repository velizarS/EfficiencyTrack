using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace EfficiencyTrack.Tests.Services.Helpers
{
    public class DailyWorkerEfficiencyCheckServiceTests : IDisposable
    {
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock = new();
        private readonly Mock<IServiceScope> _serviceScopeMock = new();
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<IEmailSender> _emailSenderMock = new();
        private readonly Mock<ILogger<DailyWorkerEfficiencyCheckService>> _loggerMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();

        private readonly EfficiencyTrackDbContext _dbContext;
        private readonly DailyWorkerEfficiencyCheckService _service;

        public DailyWorkerEfficiencyCheckServiceTests()
        {
            var options = new DbContextOptionsBuilder<EfficiencyTrackDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            // Мокваме IHttpContextAccessor, за да не хвърля NullReferenceException
            var context = new DefaultHttpContext();
            context.User = new System.Security.Claims.ClaimsPrincipal(
                new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, "TestUser")
                }));

            _httpContextAccessorMock.Setup(a => a.HttpContext).Returns(context);

            _dbContext = new EfficiencyTrackDbContext(options, _httpContextAccessorMock.Object);

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _serviceScopeFactoryMock.Setup(f => f.CreateScope()).Returns(_serviceScopeMock.Object);
            _serviceScopeMock.Setup(s => s.ServiceProvider.GetService(typeof(EfficiencyTrackDbContext)))
                .Returns(_dbContext);
            _serviceScopeMock.Setup(s => s.ServiceProvider.GetService(typeof(UserManager<ApplicationUser>)))
                .Returns(_userManagerMock.Object);
            _serviceScopeMock.Setup(s => s.ServiceProvider.GetService(typeof(IEmailSender)))
                .Returns(_emailSenderMock.Object);

            _service = new DailyWorkerEfficiencyCheckService(
                _serviceScopeFactoryMock.Object,
                _loggerMock.Object);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
        }

       

        [Fact]
        public void BuildWorkerListText_EmptyList_ReturnsOnlyHeader()
        {
            var result = _service.BuildWorkerListText(new List<Employee>());

            Assert.Contains("Следните работници нямат записи за предходния ден:", result);
            Assert.DoesNotContain("Име:", result);
        }

        [Fact]
        public void BuildWorkerListText_WithWorkers_ReturnsFormattedString()
        {
            var workers = new List<Employee>
            {
                new Employee { FirstName = "Иван", LastName = "Иванов", Code = "123" },
                new Employee { FirstName = "Петър", LastName = "Петров", Code = "456" }
            };

            var result = _service.BuildWorkerListText(workers);

            Assert.Contains("Име: Иван Иванов, Код: 123", result);
            Assert.Contains("Име: Петър Петров, Код: 456", result);
        }

        [Fact]
        public async Task CheckAndSendReportsAsync_NotYetTime_LogsAndDoesNothing()
        {
            var nowUtc = DateTime.UtcNow.Date.AddHours(6);

            await _service.CheckAndSendReportsAsync(nowUtc, CancellationToken.None);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((s, _) => s.ToString().Contains("Все още не е достигнат часът")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CheckAndSendReportsAsync_AlreadySent_LogsAndDoesNothing()
        {
            _dbContext.SentEfficiencyReports.Add(new SentEfficiencyReport { DateSent = DateTime.UtcNow.Date });
            await _dbContext.SaveChangesAsync();

            await _service.CheckAndSendReportsAsync(DateTime.UtcNow, CancellationToken.None);

            _loggerMock.Verify(l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((s, _) => s.ToString().Contains("вече е изпратен")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task CheckAndSendReportsAsync_SendsEmailsWhenNeeded()
        {
            // Arrange
            var shiftLeader = new ApplicationUser { Id = Guid.NewGuid(), Email = "leader@example.com" };

            var employees = new List<Employee>
    {
        new Employee { FirstName = "Иван", LastName = "Иванов", Code = "123", ShiftManagerUserId = shiftLeader.Id },
        new Employee { FirstName = "Петър", LastName = "Петров", Code = "456", ShiftManagerUserId = shiftLeader.Id }
    };

            var yesterday = DateTime.UtcNow.Date.AddDays(-1);

            _dbContext.Employees.AddRange(employees);
            _dbContext.DailyEfficiencies.Add(new DailyEfficiency { Date = yesterday, EmployeeId = employees[0].Id });
            await _dbContext.SaveChangesAsync();

            _userManagerMock.Setup(um => um.GetUsersInRoleAsync("ShiftLeader"))
                .ReturnsAsync(new List<ApplicationUser> { shiftLeader });

            _emailSenderMock
                .Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            var nowUtc = DateTime.UtcNow.Date.AddHours(8);

            // Act
            await _service.CheckAndSendReportsAsync(nowUtc, CancellationToken.None);

            // Assert
            _emailSenderMock.Verify(x => x.SendEmailAsync(
                shiftLeader.Email,
                It.Is<string>(s => s.Contains("Липсващи записи")),
                It.IsAny<string>()), Times.Once);

            _emailSenderMock.Verify(x => x.SendEmailAsync(
                It.Is<string>(s => s.Contains("velizar.stankov@bg.abb.com")),
                It.Is<string>(s => s.Contains("Обобщен отчет")),
                It.IsAny<string>()), Times.Once);

            _loggerMock.Setup(l => l.Log(
     It.IsAny<LogLevel>(),
     It.IsAny<EventId>(),
     It.IsAny<object>(),
     It.IsAny<Exception>(),
     It.IsAny<Func<object, Exception, string>>()))
     .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>(
         (level, eventId, state, exception, formatter) =>
         {
             Console.WriteLine(state.ToString());
         });

            Assert.True(_dbContext.SentEfficiencyReports.Any());
        }

    }
}
