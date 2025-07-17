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
using EfficiencyTrack.Services.Helpers;

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
    private readonly GreetingService _greetingService;

    public EntryController(
        IEntryService entryService,
        IEmployeeService employeeService,
        IRoutingService routingService,
        ICrudService<Shift> shiftService, GreetingService greetingService)
        : base(entryService)
    {
        _entryService = entryService;
        _employeeService = employeeService;
        _routingService = routingService;
        _shiftService = shiftService;
        _greetingService = greetingService;
    }

    protected override EntryViewModel MapToViewModel(Entry entity) => new()
    {
        Id = entity.Id,
        Date = entity.Date,
        EmployeeCode = entity.Employee?.Code ?? string.Empty,
        EmployeeName = $"{entity.Employee?.FirstName} {entity.Employee?.LastName}".Trim(),
        RoutingName = entity.Routing?.Code ?? string.Empty,
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
        EmployeeCode = entity.Employee?.Code ?? string.Empty,
        EmployeeName = $"{entity.Employee?.FirstName} {entity.Employee?.LastName}".Trim(),
        ShiftId = entity.ShiftId,
        RoutingId = entity.RoutingId,
        RoutingName = entity.Routing?.Code ?? string.Empty,
        Pieces = entity.Pieces,
        Scrap = entity.Scrap,
        WorkedMinutes = entity.WorkedMinutes,
        RequiredMinutes = (entity.Pieces + entity.Scrap) * (entity.Routing?.MinutesPerPiece ?? 0),
        EfficiencyForOperation = entity.EfficiencyForOperation
    };

    protected override Entry MapToEntity(EntryCreateViewModel model) => new()
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

    protected override Entry MapToEntity(EntryEditViewModel model) => new()
    {
        Id = model.Id,
        EmployeeId = model.EmployeeId,
        RoutingId = model.RoutingId,
        ShiftId = model.ShiftId,
        Pieces = model.Pieces,
        Scrap = model.Scrap,
        WorkedMinutes = model.WorkedMinutes
    };

    protected override EntryEditViewModel MapToEditModel(Entry entity) => new()
    {
        Id = entity.Id,
        EmployeeId = entity.EmployeeId,
        EmployeeCode = entity.Employee?.Code ?? string.Empty,
        RoutingId = entity.RoutingId,
        RoutingCode = entity.Routing?.Code ?? string.Empty,
        ShiftId = entity.ShiftId,
        Pieces = entity.Pieces,
        Scrap = entity.Scrap,
        WorkedMinutes = entity.WorkedMinutes
    };

    protected override EntryListViewModel BuildListViewModel(List<EntryViewModel> items) => new() { Entries = items };

    public override async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true)
    {
        var entries = await _entryService.GetAllWithIncludesAsync();
        var viewModels = entries.Select(MapToViewModel).ToList();
        var sortedFiltered = FilterAndSort(viewModels, searchTerm, sortBy, sortAsc);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;

        return View(BuildListViewModel(sortedFiltered));
    }

    public override async Task<IActionResult> Details(Guid id)
    {
        var entity = await _entryService.GetByIdWithIncludesAsync(id);
        if (entity == null) return NotFound();
        return View(MapToDetailModel(entity));
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
        (Employee? employee, Routing? routing) = await PrepareAndValidateEntry(model);

        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return View(model);
        }

        var entity = new Entry
        {
            Id = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            EmployeeId = employee!.Id,
            RoutingId = routing!.Id,
            ShiftId = model.ShiftId,
            Pieces = model.Pieces,
            Scrap = model.Scrap,
            WorkedMinutes = model.WorkedMinutes
        };

        try
        {
            await _entryService.AddAsync(entity);
            TempData["Message"] = await _greetingService.GetGreetingAsync(entity);
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            AddModelErrors(ex);
            await LoadSelectLists();
            return View(model);
        }
    }

    [HttpGet]
    public override async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _entryService.GetByIdWithIncludesAsync(id);
        if (entity == null) return NotFound();

        await LoadSelectLists();
        return View(MapToEditModel(entity));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(EntryEditViewModel model)
    {
        (Employee? employee, Routing? routing) = await PrepareAndValidateEntry(model);

        if (employee == null || routing == null || !ModelState.IsValid)
        {
            await LoadSelectLists();
            return View(model);
        }

        model.EmployeeId = employee.Id;
        model.RoutingId = routing.Id;

        try
        {
            await _entryService.UpdateAsync(MapToEntity(model));
            TempData["Message"] = "Записът беше успешно редактиран.";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            AddModelErrors(ex);

            await LoadSelectLists();
            return View(model);
        }
    }

    private void AddModelErrors(InvalidOperationException ex)
    {
        var errorMessages = ex.Message.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (var msg in errorMessages)
        {
            ModelState.AddModelError(string.Empty, msg);
        }
    }

    private async Task LoadSelectLists()
    {
        var shifts = await _shiftService.GetAllAsync();
        ViewBag.Shifts = shifts.Select(s => new SelectListItem
        {
            Text = s.Name,
            Value = s.Id.ToString()
        }).ToList();
    }

    protected override List<EntryViewModel> FilterAndSort(List<EntryViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.Trim();
            items = items.Where(x =>
                (!string.IsNullOrEmpty(x.EmployeeCode) && x.EmployeeCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                (!string.IsNullOrEmpty(x.RoutingName) && x.RoutingName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
            ).ToList();
        }

        items = sortBy?.ToLowerInvariant() switch
        {
            "date" => sortAsc ? items.OrderBy(x => x.Date).ToList() : items.OrderByDescending(x => x.Date).ToList(),
            "employeename" => sortAsc ? items.OrderBy(x => x.EmployeeName).ToList() : items.OrderByDescending(x => x.EmployeeName).ToList(),
            "efficiency" => sortAsc ? items.OrderBy(x => x.EfficiencyForOperation).ToList() : items.OrderByDescending(x => x.EfficiencyForOperation).ToList(),
            _ => items
        };

        return items;
    }

    private async Task<(Employee? employee, Routing? routing)> PrepareAndValidateEntry<T>(T model) where T : EntryBaseViewModel
    {
        var employee = await _employeeService.GetByCodeAsync(model.EmployeeCode);
        var routing = await _routingService.GetRoutingByCodeAsync(model.RoutingCode);

        if (employee == null)
            ModelState.AddModelError(nameof(model.EmployeeCode), "Невалиден код на служител.");
        if (routing == null)
            ModelState.AddModelError(nameof(model.RoutingCode), "Невалиден код на операция.");

        return (employee, routing);
    }

}