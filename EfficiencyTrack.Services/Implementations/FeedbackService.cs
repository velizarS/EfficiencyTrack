using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly EfficiencyTrackDbContext _context;

        public FeedbackService(EfficiencyTrackDbContext context)
        {
            _context = context;
        }

        public async Task<Feedback> CreateFeedbackAsync(Feedback feedback)
        {
            if (feedback == null)
                throw new ArgumentNullException(nameof(feedback), "Feedback cannot be null.");

            if (string.IsNullOrWhiteSpace(feedback.Message))
                throw new ArgumentException("Feedback message cannot be empty.", nameof(feedback.Message));

            if (feedback.Message.Length > 4000)
                throw new ArgumentException("Feedback message cannot exceed 4000 characters.", nameof(feedback.Message));

            feedback.Id = Guid.NewGuid();
            feedback.CreatedAt = DateTime.UtcNow;
            feedback.IsHandled = false;
            feedback.Message = feedback.Message.Trim();
            feedback.EmployeeName = feedback.EmployeeName?.ToUpperInvariant();

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return feedback;
        }

        public async Task<IEnumerable<Feedback>> GetAllFeedbacksAsync()
        {
            return await _context.Feedbacks
                .AsNoTracking()
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();
        }

        public async Task<Feedback?> GetFeedbackByIdAsync(Guid id)
        {
            return await _context.Feedbacks
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<Feedback?> ToggleHandledAsync(Guid id)
        {
            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null)
                return null;

            feedback.IsHandled = !feedback.IsHandled;

            if (feedback.IsHandled)
            {
                feedback.HandledAt = DateTime.UtcNow;
            }
            else
            {
                feedback.HandledAt = null;
            }

            await _context.SaveChangesAsync();
            return feedback;
        }


        public async Task<bool> DeleteFeedbackAsync(Guid id)
        {
            var feedback = await _context.Feedbacks.FirstOrDefaultAsync(f => f.Id == id);
            if (feedback == null)
                return false;

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
