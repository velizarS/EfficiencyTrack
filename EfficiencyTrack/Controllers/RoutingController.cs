using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.Routing;
using EfficiencyTrack.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class RoutingController : BaseCrudController<
    Routing,
    RoutingViewModel,
    RoutingListViewModel,
    RoutingCreateViewModel,
    RoutingEditViewModel,
    RoutingDetailViewModel>
{
    private readonly IRoutingService _routingService;
    private readonly ICrudService<Department> _departmentService;

    public RoutingController(
        IRoutingService routingService,
        ICrudService<Department> departmentService)
        : base(routingService)
    {
        _routingService = routingService;
        _departmentService = departmentService;
    }

    protected override RoutingViewModel MapToViewModel(Routing entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        MinutesPerPiece = entity.MinutesPerPiece,
        DepartmentName = entity.Department?.Name ?? "—",
        Zone = entity.Zone
    };

    protected override RoutingDetailViewModel MapToDetailModel(Routing entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Description = entity.Description,
        Zone = entity.Zone,
        MinutesPerPiece = entity.MinutesPerPiece,
        DepartmentId = entity.DepartmentId,
        DepartmentName = entity.Department?.Name ?? "—"
    };

    protected override Routing MapToEntity(RoutingCreateViewModel model) => new()
    {
        Code = model.Code,
        Description = model.Description,
        Zone = model.Zone,
        MinutesPerPiece = model.MinutesPerPiece,
        DepartmentId = model.DepartmentId
    };

    protected override Routing MapToEntity(RoutingEditViewModel model) => new()
    {
        Id = model.Id,
        Code = model.Code,
        Description = model.Description,
        Zone = model.Zone,
        MinutesPerPiece = model.MinutesPerPiece,
        DepartmentId = model.DepartmentId
    };

    protected override RoutingEditViewModel MapToEditModel(Routing entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        Description = entity.Description,
        Zone = entity.Zone,
        MinutesPerPiece = entity.MinutesPerPiece,
        DepartmentId = entity.DepartmentId
    };

    protected override RoutingListViewModel BuildListViewModel(List<RoutingViewModel> items) => new()
    {
        Routings = items
    };

    protected async Task PrepareDropdownsAsync(object? model = null)
    {
        var departments = await _departmentService.GetAllAsync();

        var departmentItems = departments
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name })
            .ToList();

        var zoneItems = new List<SelectListItem>
        {
            new() { Value = "Zone A", Text = "Zone A" },
            new() { Value = "Zone B", Text = "Zone B" },
            new() { Value = "Zone C", Text = "Zone C" }
        };

        switch (model)
        {
            case RoutingCreateViewModel createModel:
                createModel.AvailableDepartments = departmentItems;
                createModel.AvailableZones = zoneItems;
                break;
            case RoutingEditViewModel editModel:
                editModel.AvailableDepartments = departmentItems;
                editModel.AvailableZones = zoneItems;
                break;
        }
    }

    public override async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true)
    {
        var entities = await _routingService.GetAllWithDepartmentsAsync();
        var viewModels = entities.Select(MapToViewModel).ToList();

        var filteredSorted = FilterAndSort(viewModels, searchTerm, sortBy, sortAsc);
        var listModel = BuildListViewModel(filteredSorted);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;

        return View(listModel);
    }


    public override async Task<IActionResult> Details(Guid id)
    {
        var entity = await _routingService.GetByIdWithDepartmentAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var model = MapToDetailModel(entity);
        return View(model);
    }

    public override async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _routingService.GetByIdWithDepartmentAsync(id);
        if (entity == null)
        {
            return NotFound();
        }

        var model = MapToEditModel(entity);
        await PrepareDropdownsAsync(model);
        return View(model);
    }

    public override async Task<IActionResult> Create()
    {
        var model = new RoutingCreateViewModel();
        await PrepareDropdownsAsync(model);
        return View(model);
    }

    protected override List<RoutingViewModel> FilterAndSort(List<RoutingViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            items = items
                .Where(x =>
                    (x.Code?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (x.DepartmentName?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false))
                .ToList();
        }

        items = sortBy switch
        {
            "code" => sortAsc
                ? items.OrderBy(x => x.Code).ToList()
                : items.OrderByDescending(x => x.Code).ToList(),

            "department" => sortAsc
                ? items.OrderBy(x => x.DepartmentName).ToList()
                : items.OrderByDescending(x => x.DepartmentName).ToList(),

            "minutes" => sortAsc
                ? items.OrderBy(x => x.MinutesPerPiece).ToList()
                : items.OrderByDescending(x => x.MinutesPerPiece).ToList(),

            "zone" => sortAsc
                ? items.OrderBy(x => x.Zone).ToList()
                : items.OrderByDescending(x => x.Zone).ToList(),

            _ => items
        };

        return items;
    }
}
