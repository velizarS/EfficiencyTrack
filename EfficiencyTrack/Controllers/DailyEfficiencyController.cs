using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.DailyEfficiencyViewModels;
using EfficiencyTrack.ViewModels.EntryViewModel;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyTrack.Web.Controllers
{
    public class DailyEfficiencyController : Controller
    {
        private readonly IDailyEfficiencyService _dailyEfficiencyService;

        public DailyEfficiencyController(IDailyEfficiencyService dailyEfficiencyService)
        {
            _dailyEfficiencyService = dailyEfficiencyService;
        }

        public async Task<IActionResult> Index()
        {
            var efficiencies = await _dailyEfficiencyService.GetAllAsync();

            var viewModel = new DailyEfficiencyListViewModel
            {
                DailyEfficiencies = efficiencies.Select(e => new DailyEfficiencyViewModel
                {
                    Id = e.Id,
                    Date = e.Date,
                    EmployeeCode = e.Employee?.Code ?? "N/A",
                    EmployeeFullName = string.Join(" ", e.Employee?.FirstName, e.Employee?.MiddleName, e.Employee?.LastName).Trim(),
                    TotalWorkedMinutes = e.TotalWorkedMinutes,
                    ShiftName = e.Shift?.Name ?? "N/A",
                    EfficiencyPercentage = e.EfficiencyPercentage
                }).ToList()
            };

            return View(viewModel);
        }

        // GET: DailyEfficiency/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            var dto = await _dailyEfficiencyService.GetByIdAsync(id);
            if (dto == null)
                return NotFound();

            var viewModel = new DailyDetailEfficiencyViewModel
            {
                Id = dto.Id,
                Date = dto.Date,
                EmployeeCode = dto.EmployeeCode,
                EmployeeFullName = dto.EmployeeFullName,
                TotalWorkedMinutes = dto.TotalWorkedMinutes,
                ShiftName = dto.ShiftName,
                EfficiencyPercentage = dto.EfficiencyPercentage,
                DetailEntries = dto.Entries.Select(e => new EntryDetailsViewModel
                {
                    Date = e.Date,
                    EmployeeId = e.EmployeeId,
                    RoutingId = e.RoutingId,
                    RoutingName = e.RoutingName,
                    EfficiencyForOperation = e.EfficiencyForOperation
                }).ToList()
            };

            return View(viewModel);
        }
    }
}
