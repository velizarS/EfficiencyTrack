using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.EntryViewModel;
using EfficiencyTrack.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using EfficiencyTrack.ViewModels.Routing;

public class EntryController : BaseCrudController<
    Entry,
    EntryViewModel,
    EntryListViewModel,
    EntryCreateViewModel,
    EntryEditViewModel,
    EntryDetailsViewModel>
{
    private readonly IEntryService _entryService;
    private readonly IEmployeeService _employeeService;
    private readonly IRoutingService _routingService;
    private readonly ICrudService<Shift> _shiftService;

    public EntryController(
        IEntryService entryService,
        IEmployeeService employeeService,
        IRoutingService routingService,
        ICrudService<Shift> shiftService)
        : base(entryService)
    {
        _entryService = entryService;
        _employeeService = employeeService;
        _routingService = routingService;
        _shiftService = shiftService;
    }

    protected override EntryViewModel MapToViewModel(Entry entity) => new()
    {
        Id = entity.Id,
        Date = entity.Date,
        EmployeeCode = entity.Employee?.Code ?? "",
        EmployeeName = $"{entity.Employee?.FirstName ?? ""} {entity.Employee?.LastName ?? ""}".Trim(),
        RoutingName = entity.Routing?.Code ?? "",
        Pieces = entity.Pieces,
        Scrap = entity.Scrap,
        WorkedMinutes = entity.WorkedMinutes,
        EfficiencyForOperation = entity.EfficiencyForOperation
    };

    protected override EntryDetailsViewModel MapToDetailModel(Entry entity) => new()
    {
        Id = entity.Id,
        Date = entity.Date,
        EmployeeId = entity.EmployeeId,
        EmployeeCode = entity.Employee?.Code ?? "",
        EmployeeName = $"{entity.Employee?.FirstName ?? ""} {entity.Employee?.LastName ?? ""}".Trim(),
        ShiftId = entity.ShiftId,
        RoutingId = entity.RoutingId,
        RoutingName = entity.Routing?.Code ?? "",
        Pieces = entity.Pieces,
        Scrap = entity.Scrap,
        WorkedMinutes = entity.WorkedMinutes,
        RequiredMinutes = (entity.Pieces + entity.Scrap) * (entity.Routing?.MinutesPerPiece ?? 0),
        EfficiencyForOperation = entity.EfficiencyForOperation
    };

    protected override Entry MapToEntity(EntryCreateViewModel model)
    {
        return new Entry
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            EmployeeId = model.EmployeeId,
            RoutingId = model.RoutingId,
            ShiftId = model.ShiftId,
            Pieces = model.Pieces,
            Scrap = model.Scrap,
            WorkedMinutes = model.WorkedMinutes
        };
    }

    protected override Entry MapToEntity(EntryEditViewModel model)
    {
        return new Entry
        {
            Id = model.Id,
            EmployeeId = model.EmployeeId,
            RoutingId = model.RoutingId,
            ShiftId = model.ShiftId,
            Pieces = model.Pieces,
            Scrap = model.Scrap,
            WorkedMinutes = model.WorkedMinutes
        };
    }

    protected override EntryEditViewModel MapToEditModel(Entry entity)
    {
        return new EntryEditViewModel
        {
            Id = entity.Id,
            EmployeeId = entity.EmployeeId,
            EmployeeCode = entity.Employee?.Code ?? "",
            RoutingId = entity.RoutingId,
            RoutingCode = entity.Routing?.Code ?? "",
            ShiftId = entity.ShiftId,
            Pieces = entity.Pieces,
            Scrap = entity.Scrap,
            WorkedMinutes = entity.WorkedMinutes
        };
    }

    protected override EntryListViewModel BuildListViewModel(List<EntryViewModel> items)
    {
        return new EntryListViewModel { Entries = items };
    }

    public override async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true)
    {
        var entries = await _entryService.GetAllWithIncludesAsync(); 
        var viewModels = entries.Select(MapToViewModel).ToList();
        var sortedFiltered = FilterAndSort(viewModels, searchTerm, sortBy, sortAsc);
        var listViewModel = BuildListViewModel(sortedFiltered);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;

        return View(listViewModel);
    }

    public override async Task<IActionResult> Details(Guid id)
    {
        var entity = await _entryService.GetByIdWithIncludesAsync(id); 
        if (entity == null) return NotFound();
        var viewModel = MapToDetailModel(entity);
        return View(viewModel);
    }

    [HttpGet]
    public override async Task<IActionResult> Create()
    {
        await LoadSelectLists();
        return View(new EntryCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create(EntryCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return View(model);
        }

        var employee = await _employeeService.GetByCodeAsync(model.EmployeeCode);
        var routing = await _routingService.GetRoutingByCodeAsync(model.RoutingCode);

        if (employee == null)
            ModelState.AddModelError(nameof(model.EmployeeCode), "Невалиден код на служител.");
        if (routing == null)
            ModelState.AddModelError(nameof(model.RoutingCode), "Невалиден код на операция.");

        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return View(model);
        }

        var entity = new Entry
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            EmployeeId = employee.Id,
            RoutingId = routing.Id,
            ShiftId = model.ShiftId,
            Pieces = model.Pieces,
            Scrap = model.Scrap,
            WorkedMinutes = model.WorkedMinutes
        };

        await _entryService.AddAsync(entity);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public override async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _entryService.GetByIdWithIncludesAsync(id);
        if (entity == null) return NotFound();

        var model = MapToEditModel(entity);
        await LoadSelectLists();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(EntryEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return View(model);
        }

        var entity = MapToEntity(model);
        await _entryService.UpdateAsync(entity);
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadSelectLists()
    {
        var shifts = await _shiftService.GetAllAsync();

        ViewBag.Shifts = shifts.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString()
        });
    }

    protected override List<EntryViewModel> FilterAndSort(List<EntryViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            items = items
                .Where(x =>
                    (x.EmployeeCode?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.RoutingName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        items = sortBy switch
        {
            "date" => sortAsc
               ? items.OrderBy(x => x.Date).ToList()
               : items.OrderByDescending(x => x.Date).ToList(),

            "employeeName" => sortAsc
                ? items.OrderBy(x => x.EmployeeName).ToList()
                : items.OrderByDescending(x => x.EmployeeName).ToList(),

           
            "efficiency" => sortAsc
                ? items.OrderBy(x => x.EfficiencyForOperation).ToList()
                : items.OrderByDescending(x => x.EfficiencyForOperation).ToList(),

            _ => items
        };

        return items;
    }
}
