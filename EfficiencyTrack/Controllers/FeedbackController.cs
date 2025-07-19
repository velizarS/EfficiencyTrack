using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.FeedbackViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EfficiencyTrack.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _service;

        public FeedbackController(IFeedbackService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true)
        {
            IQueryable<Feedback> query = _service.GetFilteredFeedbacks(searchTerm, sortBy, sortAsc);

            List<Feedback> feedbackEntities = await query.ToListAsync();

            List<FeedbackViewModel> feedbacks = feedbackEntities
                .Select(f => new FeedbackViewModel
                {
                    Id = f.Id,
                    EmployeeName = f.EmployeeName ?? string.Empty,
                    CreatedAt = f.CreatedAt,
                    Message = f.Message,
                    IsHandled = f.IsHandled
                })
                .ToList();

            FeedbackListViewModel viewModel = new()
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
            Feedback? updated = await _service.ToggleHandledAsync(id);
            return updated == null ? NotFound() : RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(Guid id)
        {
            Feedback? feedback = await _service.GetFeedbackByIdAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            FeedbackDetailViewModel viewModel = new()
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

        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]  
        public async Task<IActionResult> Create(FeedbackCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            Feedback entity = new()
            {
                EmployeeName = model.EmployeeName,
                Message = model.Message
            };

            await _service.CreateFeedbackAsync(entity);

            return RedirectToAction("Index", "Home");
        }


        public async Task<IActionResult> Delete(Guid id)
        {
            Feedback? feedback = await _service.GetFeedbackByIdAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }

            FeedbackDetailViewModel viewModel = new()
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
            _ = await _service.DeleteFeedbackAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
