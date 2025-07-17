using EfficiencyTrack.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<Feedback> CreateFeedbackAsync(Feedback feedback);
        Task<IEnumerable<Feedback>> GetAllFeedbacksAsync();
        Task<Feedback?> GetFeedbackByIdAsync(Guid id);
        Task<Feedback?> ToggleHandledAsync(Guid id);
        Task<bool> DeleteFeedbackAsync(Guid id);
        IQueryable<Feedback> GetFilteredFeedbacks(string? searchTerm, string? sortBy, bool sortAsc);
    }
}
