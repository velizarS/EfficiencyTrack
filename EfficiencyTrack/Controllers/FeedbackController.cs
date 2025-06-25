using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        public async Task<IActionResult> Index()
        {
            var feedbacks = await _service.GetAllFeedbacksAsync();

            var viewModel = new FeedbackListViewModel
            {
                Feedbacks = feedbacks.Select(f => new FeedbackViewModel
                {
                    EmployeeName = f.EmployeeName,
                    CreatedAt = f.CreatedAt,
                    Message = f.Message
                }).ToList()
            };

            return View(viewModel);
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
