using EfficiencyTrack.Controllers;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.EntryViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize]
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
    private readonly IGreetingService _greetingService;

    public EntryController(
        IEntryService entryService,
        IEmployeeService employeeService,
        IRoutingService routingService,
        ICrudService<Shift> shiftService,
        IGreetingService greetingService)
        : base(entryService)
    {
        _entryService = entryService;
        _employeeService = employeeService;
        _routingService = routingService;
        _shiftService = shiftService;
        _greetingService = greetingService;
    }

    protected override EntryViewModel MapToViewModel(Entry entity)
    {
        return new()
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
    }

    protected override EntryDetailsViewModel MapToDetailModel(Entry entity)
    {
        return new()
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
    }

    protected override Entry MapToEntity(EntryCreateViewModel model)
    {
        return new()
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
        return new()
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
        return new()
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
    }

    protected override EntryListViewModel BuildListViewModel(List<EntryViewModel> items)
    {
        return new() { Entries = items };
    }

    public override async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true, int page = 1, int pageSize = 20)
    {
        List<Entry> entries = await _entryService.GetAllWithIncludesAsync();
        List<EntryViewModel> viewModels = entries.Select(MapToViewModel).ToList();
        List<EntryViewModel> filteredSorted = FilterAndSort(viewModels, searchTerm, sortBy, sortAsc);

        List<EntryViewModel> pagedItems = filteredSorted
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalCount = filteredSorted.Count;

        return View(BuildListViewModel(pagedItems));
    }

    public override async Task<IActionResult> Details(Guid id)
    {
        Entry? entity = await _entryService.GetByIdWithIncludesAsync(id);
        return entity == null ? NotFound() : View(MapToDetailModel(entity));
    }

    [HttpGet]
    [AllowAnonymous]
    public override async Task<IActionResult> Create()
    {
        await LoadSelectLists();
        return View(new EntryCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public override async Task<IActionResult> Create(EntryCreateViewModel model)
    {
        (Employee? employee, Routing? routing) = await PrepareAndValidateEntry(model);

        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return View(model);
        }

        Entry entity = new()
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
            TempData["Message"] = await _greetingService.GetGreetingMessageAsync(entity);
            return RedirectToAction("Index", "Home");
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
        Entry? entity = await _entryService.GetByIdWithIncludesAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

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
        string[] errorMessages = ex.Message.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        foreach (string msg in errorMessages)
        {
            ModelState.AddModelError(string.Empty, msg);
        }
    }

    private async Task LoadSelectLists()
    {
        IEnumerable<Shift> shifts = await _shiftService.GetAllAsync();
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
        Employee? employee = await _employeeService.GetByCodeAsync(model.EmployeeCode);
        Routing? routing = await _routingService.GetRoutingByCodeAsync(model.RoutingCode);

        if (employee == null)
        {
            ModelState.AddModelError(nameof(model.EmployeeCode), "Невалиден код на служител.");
        }

        if (routing == null)
        {
            ModelState.AddModelError(nameof(model.RoutingCode), "Невалиден код на операция.");
        }

        return (employee, routing);
    }
}
