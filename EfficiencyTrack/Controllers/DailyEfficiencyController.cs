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

        private DailyEfficiencyViewModel MapToViewModel(Data.Models.DailyEfficiency entity)
        {
            return new DailyEfficiencyViewModel
            {
                Id = entity.Id,
                Date = entity.Date,
                EmployeeCode = entity.Employee?.Code ?? "N/A",
                EmployeeFullName = $"{entity.Employee?.FirstName ?? ""} {entity.Employee?.LastName ?? ""}".Trim(),
                TotalWorkedMinutes = entity.TotalWorkedMinutes,
                ShiftName = entity.Shift?.Name ?? "N/A",
                EfficiencyPercentage = entity.EfficiencyPercentage
            };
        }

        public async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true, int page = 1, int pageSize = 20)
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

            var viewModels = efficiencies.Select(MapToViewModel).ToList();

            viewModels = FilterAndSort(viewModels, searchTerm, sortBy, sortAsc);

            int totalCount = viewModels.Count();

            var pagedViewModels = viewModels
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var listModel = new DailyEfficiencyListViewModel
            {
                DailyEfficiencies = pagedViewModels,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAsc = sortAsc;

            return View(listModel);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var dto = await _dailyEfficiencyService.GetByIdAsync(id);
            if (dto == null)
            {
                return NotFound();
            }

            var viewModel = new DailyDetailEfficiencyViewModel
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

                _ => items.OrderByDescending(x => x.Date).ToList(), // default sort
            };

            return items;
        }
    }
}
