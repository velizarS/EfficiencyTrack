using System;
using System.Linq;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EfficiencyTrack.Tests.ServicesTests.mainServicesTests
{
    public class FeedbackServiceTests
    {
        private EfficiencyTrackDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<EfficiencyTrackDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new EfficiencyTrackDbContext(options, null!); // null за IHttpContextAccessor, ако не ти трябва в теста
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldCreateFeedbackSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new FeedbackService(context);

            var feedback = new Feedback
            {
                Message = "Test feedback message",
                EmployeeName = "ivan"
            };

            // Act
            var result = await service.CreateFeedbackAsync(feedback);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("ivan".ToUpperInvariant(), result.EmployeeName);
            Assert.False(result.IsHandled);
            Assert.True(result.CreatedAt <= DateTime.UtcNow);
            Assert.Equal("Test feedback message", result.Message);
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldThrowArgumentNullException_WhenFeedbackIsNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new FeedbackService(context);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateFeedbackAsync(null!));
        }

        [Fact]
        public async Task CreateFeedbackAsync_ShouldThrowArgumentException_WhenMessageIsEmpty()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new FeedbackService(context);

            var feedback = new Feedback
            {
                Message = "   "
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateFeedbackAsync(feedback));
        }

        [Fact]
        public async Task GetAllFeedbacksAsync_ShouldReturnAllFeedbacks()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Feedbacks.Add(new Feedback { Id = Guid.NewGuid(), Message = "A", CreatedAt = DateTime.UtcNow.AddDays(-1) });
            context.Feedbacks.Add(new Feedback { Id = Guid.NewGuid(), Message = "B", CreatedAt = DateTime.UtcNow });
            await context.SaveChangesAsync();

            var service = new FeedbackService(context);

            // Act
            var feedbacks = await service.GetAllFeedbacksAsync();

            // Assert
            Assert.Equal(2, feedbacks.Count());
            Assert.True(feedbacks.First().CreatedAt >= feedbacks.Last().CreatedAt); // Проверява сортирането
        }

        [Fact]
        public async Task ToggleHandledAsync_ShouldToggleIsHandled()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var feedback = new Feedback { Id = Guid.NewGuid(), Message = "Test", IsHandled = false };
            context.Feedbacks.Add(feedback);
            await context.SaveChangesAsync();

            var service = new FeedbackService(context);

            // Act
            var toggled = await service.ToggleHandledAsync(feedback.Id);

            // Assert
            Assert.NotNull(toggled);
            Assert.True(toggled!.IsHandled);
            Assert.NotNull(toggled.HandledAt);

            // Act again to untoggle
            var toggledBack = await service.ToggleHandledAsync(feedback.Id);

            Assert.NotNull(toggledBack);
            Assert.False(toggledBack!.IsHandled);
            Assert.Null(toggledBack.HandledAt);
        }

        [Fact]
        public async Task DeleteFeedbackAsync_ShouldDeleteFeedback()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var feedback = new Feedback { Id = Guid.NewGuid(), Message = "To delete" };
            context.Feedbacks.Add(feedback);
            await context.SaveChangesAsync();

            var service = new FeedbackService(context);

            // Act
            var deleted = await service.DeleteFeedbackAsync(feedback.Id);

            // Assert
            Assert.True(deleted);
            Assert.Null(await context.Feedbacks.FindAsync(feedback.Id));
        }

        [Fact]
        public void GetFilteredFeedbacks_ShouldFilterAndSort()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Feedbacks.Add(new Feedback
            {
                Id = Guid.NewGuid(),
                EmployeeName = "Alice",
                Message = "Test message A",
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                IsHandled = false
            });
            context.Feedbacks.Add(new Feedback
            {
                Id = Guid.NewGuid(),
                EmployeeName = "Bob",
                Message = "Test message B",
                CreatedAt = DateTime.UtcNow,
                IsHandled = true
            });
            context.Feedbacks.Add(new Feedback
            {
                Id = Guid.NewGuid(),
                EmployeeName = "Charlie",
                Message = "Test message C",
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                IsHandled = false
            });
            context.SaveChanges();

            var service = new FeedbackService(context);

            // Act
            var filtered = service.GetFilteredFeedbacks("bo", "name", true).ToList();

            // Assert
            Assert.Single(filtered);
            Assert.Equal("Bob", filtered[0].EmployeeName);

            var sorted = service.GetFilteredFeedbacks(null, "date", false).ToList();
            Assert.Equal(3, sorted.Count);
            Assert.True(sorted[0].CreatedAt >= sorted[1].CreatedAt);
        }

    }
}
