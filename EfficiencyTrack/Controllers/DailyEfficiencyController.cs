using EfficiencyTrack.Services.DTOs.EfficiencyTrack.Services.DTOs;
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

        public async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true)
        {
            var efficiencies = await _dailyEfficiencyService.GetAllAsync();

            var viewModels = efficiencies.Select(e => new DailyEfficiencyViewModel
            {
                Id = e.Id,
                Date = e.Date,
                EmployeeCode = e.Employee?.Code ?? "N/A",
                EmployeeFullName = string.Join(" ", e.Employee?.FirstName, e.Employee?.MiddleName, e.Employee?.LastName).Trim(),
                TotalWorkedMinutes = e.TotalWorkedMinutes,
                ShiftName = e.Shift?.Name ?? "N/A",
                EfficiencyPercentage = e.EfficiencyPercentage
            }).ToList();

            var filteredSorted = FilterAndSort(viewModels, searchTerm, sortBy, sortAsc);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAsc = sortAsc;

            var listViewModel = new DailyEfficiencyListViewModel
            {
                DailyEfficiencies = filteredSorted
            };

            return View(listViewModel);
        }


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

        protected List<DailyEfficiencyViewModel> FilterAndSort(List<DailyEfficiencyViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                items = items
                    .Where(x => x.EmployeeCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            items = sortBy?.ToLower() switch
            {
                "date" => sortAsc
                    ? items.OrderBy(x => x.Date).ToList()
                    : items.OrderByDescending(x => x.Date).ToList(),

                "employee" => sortAsc
                    ? items.OrderBy(x => x.EmployeeFullName).ToList()
                    : items.OrderByDescending(x => x.EmployeeFullName).ToList(),

                "minutes" => sortAsc
                    ? items.OrderBy(x => x.TotalWorkedMinutes).ToList()
                    : items.OrderByDescending(x => x.TotalWorkedMinutes).ToList(),

                "shift" => sortAsc
                    ? items.OrderBy(x => x.ShiftName).ToList()
                    : items.OrderByDescending(x => x.ShiftName).ToList(),

                "efficiency" => sortAsc
                    ? items.OrderBy(x => x.EfficiencyPercentage).ToList()
                    : items.OrderByDescending(x => x.EfficiencyPercentage).ToList(),

                _ => items
            };

            return items;
        }

    }
}
