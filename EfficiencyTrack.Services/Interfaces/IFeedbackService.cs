﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<Feedback> CreateFeedbackAsync(Feedback feedback);

        Task<IEnumerable<Feedback>> GetAllFeedbacksAsync();

        Task<Feedback?> GetFeedbackByIdAsync(Guid id);

        Task<Feedback?> MarkAsHandledAsync(Guid id);

        Task<bool> DeleteFeedbackAsync(Guid id);
    }

}
