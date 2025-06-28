using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.FeedbackViewModels;

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
            var feedbacks = await _service.GetAllFeedbacksAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                feedbacks = feedbacks.Where(f => f.EmployeeName != null && f.EmployeeName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            feedbacks = sortBy switch
            {
                "name" => sortAsc
                    ? feedbacks.OrderBy(f => f.EmployeeName)
                    : feedbacks.OrderByDescending(f => f.EmployeeName),
                "date" => sortAsc
                    ? feedbacks.OrderBy(f => f.CreatedAt)
                    : feedbacks.OrderByDescending(f => f.CreatedAt),
                "handled" => sortAsc
                    ? feedbacks.OrderBy(f => f.IsHandled) 
                    : feedbacks.OrderByDescending(f => f.IsHandled),
                _ => feedbacks.OrderByDescending(f => f.CreatedAt),
            };

            var viewModel = new FeedbackListViewModel
            {
                Feedbacks = feedbacks.Select(f => new FeedbackViewModel
                {
                    Id = f.Id,
                    EmployeeName = f.EmployeeName,
                    CreatedAt = f.CreatedAt,
                    Message = f.Message,
                    IsHandled = f.IsHandled
                }).ToList()
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
            {
                return NotFound();
            }

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
            {
                return View(model);
            }

            var entity = new Feedback
            {
                Id = Guid.NewGuid(),
                EmployeeName = model.EmployeeName,
                Message = model.Message,
                CreatedAt = DateTime.UtcNow,
                IsHandled = false
            };

            await _service.CreateFeedbackAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var feedback = await _service.GetFeedbackByIdAsync(id);
            if (feedback == null) return NotFound();

            return View(new FeedbackDetailViewModel
            {
                Id = feedback.Id,
                EmployeeName = feedback.EmployeeName ?? string.Empty,
                Message = feedback.Message,
                CreatedAt = feedback.CreatedAt,
                IsHandled = feedback.IsHandled,
                HandledAt = feedback.HandledAt
            });
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
