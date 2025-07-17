using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.FeedbackViewModels;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Web.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _service;

        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true)
        {
            var query = _service.GetFilteredFeedbacks(searchTerm, sortBy, sortAsc);

            var feedbackEntities = await query.ToListAsync();

            var feedbacks = feedbackEntities
                .Select(f => new FeedbackViewModel
                {
                    Id = f.Id,
                    EmployeeName = f.EmployeeName ?? string.Empty,
                    CreatedAt = f.CreatedAt,
                    Message = f.Message,
                    IsHandled = f.IsHandled
                })
                .ToList();

            var viewModel = new FeedbackListViewModel
            {
                Feedbacks = feedbacks
            };

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAsc = sortAsc;

            return View(viewModel);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHandled(Guid id)
        {
            var updated = await _service.ToggleHandledAsync(id);
            if (updated == null)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var feedback = await _service.GetFeedbackByIdAsync(id);
            if (feedback == null) return NotFound();

            var viewModel = new FeedbackDetailViewModel
            {
                Id = feedback.Id,
                EmployeeName = feedback.EmployeeName ?? string.Empty,
                Message = feedback.Message,
                CreatedAt = feedback.CreatedAt,
                IsHandled = feedback.IsHandled,
                HandledAt = feedback.HandledAt
            };

            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedbackCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = new Feedback
            {
                EmployeeName = model.EmployeeName,
                Message = model.Message
            };

            await _service.CreateFeedbackAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var feedback = await _service.GetFeedbackByIdAsync(id);
            if (feedback == null) return NotFound();

            var viewModel = new FeedbackDetailViewModel
            {
                Id = feedback.Id,
                EmployeeName = feedback.EmployeeName ?? string.Empty,
                Message = feedback.Message,
                CreatedAt = feedback.CreatedAt,
                IsHandled = feedback.IsHandled,
                HandledAt = feedback.HandledAt
            };

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            await _service.DeleteFeedbackAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
