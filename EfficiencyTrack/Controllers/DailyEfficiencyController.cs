using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.DailyEfficiencyViewModels;
using EfficiencyTrack.ViewModels.EntryViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Data.Identity;

namespace EfficiencyTrack.Controllers
{
    public class DailyEfficiencyController : Controller
    {
        private readonly IDailyEfficiencyService _dailyEfficiencyService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DailyEfficiencyController(
            IDailyEfficiencyService dailyEfficiencyService,
            UserManager<ApplicationUser> userManager)
        {
            _dailyEfficiencyService = dailyEfficiencyService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true)
        {
            IEnumerable<Data.Models.DailyEfficiency> efficiencies;

            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                efficiencies = await _dailyEfficiencyService.GetAllAsync();
            }
            else if (User.IsInRole("ShiftLeader"))
            {
                var user = await _userManager.GetUserAsync(User);
                efficiencies = user != null
                    ? await _dailyEfficiencyService.GetByShiftManagerIdAsync(user.Id)
                    : Enumerable.Empty<Data.Models.DailyEfficiency>();
            }
            else 
            {
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    efficiencies = await _dailyEfficiencyService.GetByEmployeeCodeAsync(searchTerm);
                }
                else
                {
                    efficiencies = Enumerable.Empty<Data.Models.DailyEfficiency>();
                    ViewBag.Warning = "Моля, въведете служебния си код за достъп.";
                }
            }

            List<DailyEfficiencyViewModel> viewModels = efficiencies.Select(e => new DailyEfficiencyViewModel
            {
                Id = e.Id,
                Date = e.Date,
                EmployeeCode = e.Employee?.Code ?? "N/A",
                EmployeeFullName = string.Join(" ", e.Employee?.FirstName, e.Employee?.MiddleName, e.Employee?.LastName).Trim(),
                TotalWorkedMinutes = e.TotalWorkedMinutes,
                ShiftName = e.Shift?.Name ?? "N/A",
                EfficiencyPercentage = e.EfficiencyPercentage
            }).ToList();

            List<DailyEfficiencyViewModel> filteredSorted = FilterAndSort(viewModels, searchTerm, sortBy, sortAsc);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAsc = sortAsc;

            DailyEfficiencyListViewModel listViewModel = new()
            {
                DailyEfficiencies = filteredSorted
            };

            return View(listViewModel);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var dto = await _dailyEfficiencyService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            DailyDetailEfficiencyViewModel viewModel = new()
            {
                Id = dto.Id,
                Date = dto.Date,
                EmployeeCode = dto.EmployeeCode,
                EmployeeFullName = dto.EmployeeFullName,
                TotalWorkedMinutes = dto.TotalWorkedMinutes,
                TotalNeededMinutes = dto.TotalNeddedMinutes,
                ShiftName = dto.ShiftName,
                EfficiencyPercentage = dto.EfficiencyPercentage,
                DetailEntries = dto.Entries.Select(e => new EntryDetailsViewModel
                {
                    Date = e.Date,
                    EmployeeId = e.EmployeeId,
                    RoutingId = e.RoutingId,
                    RoutingName = e.RoutingName!,
                    Pieces = e.Pieces,
                    WorkedMinutes = e.WorkedMinutes,
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

                _ => items.OrderByDescending(x => x.Date).ToList(),
            };

            return items;
        }
    }
}
