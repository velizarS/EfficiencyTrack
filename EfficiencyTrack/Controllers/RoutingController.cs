using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.Routing;
using EfficiencyTrack.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

public class RoutingController : BaseCrudController<
    Routing,
    RoutingViewModel,
    RoutingListViewModel,
    RoutingCreateViewModel,
    RoutingEditViewModel,
    RoutingDetailViewModel>
{
    private readonly ICrudService<Department> _departmentService;

    public RoutingController(
        ICrudService<Routing> routingService,
        ICrudService<Department> departmentService)
        : base(routingService)
    {
        _departmentService = departmentService;
    }

    protected override RoutingViewModel MapToViewModel(Routing entity) => new()
    {
        Id = entity.Id,
        Code = entity.Code,
        MinutesPerPiece = entity.MinutesPerPiece
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

    protected  async Task PrepareDropdownsAsync(object? model = null)
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
                .Where(x => x.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        items = sortBy switch
        {
            "code" => sortAsc
                ? items.OrderBy(x => x.Code).ToList()
                : items.OrderByDescending(x => x.Code).ToList(),

            "minutes" => sortAsc
                ? items.OrderBy(x => x.MinutesPerPiece).ToList()
                : items.OrderByDescending(x => x.MinutesPerPiece).ToList(),

            _ => items
        };

        return items;
    }

}
