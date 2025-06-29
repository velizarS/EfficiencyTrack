using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.Department;
using EfficiencyTrack.ViewModels.DepartmentViewModels;
using EfficiencyTrack.ViewModels.DepartmentViewModels.EfficiencyTrack.ViewModels.Department;
using EfficiencyTrack.ViewModels.Routing;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyTrack.Web.Controllers;

public class DepartmentController : BaseCrudController<
    Department,
    DepartmentViewModel,
    DepartmentListViewModel,
    DepartmentCreateViewModel,
    DepartmentEditViewModel,
    DepartmentDetailViewModel>
{
    private readonly IDepartmentService _departmentService;

    public DepartmentController(
        ICrudService<Department> service,
        IDepartmentService departmentService)
        : base(service)
    {
        _departmentService = departmentService;
    }

    protected override DepartmentViewModel MapToViewModel(Department entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
        };

    protected override DepartmentDetailViewModel MapToDetailModel(Department entity)
    {
        return new DepartmentDetailViewModel
        {
            Id = entity.Id,
            Name = entity.Name,
            EmployeeNames = entity.Employees
                .Select(e => $"{e.FirstName} {e.LastName}")
                .ToList()
        };
    }

    public override async Task<IActionResult> Details(Guid id)
    {
        var departmentWithEmployees = await _departmentService.GetDepartmentWithEmployeesAsync(id);

        if (departmentWithEmployees == null)
            return NotFound();

        var viewModel = MapToDetailModel(departmentWithEmployees);

        return View(viewModel);
    }

    protected override Department MapToEntity(DepartmentCreateViewModel model)
        => new()
        {
            Name = model.Name
        };

    protected override Department MapToEntity(DepartmentEditViewModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name
        };

    protected override DepartmentEditViewModel MapToEditModel(Department entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name
        };

    protected override DepartmentListViewModel BuildListViewModel(List<DepartmentViewModel> items)
        => new()
        {
            Departments = items
        };


    protected override List<DepartmentViewModel> FilterAndSort(List<DepartmentViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            items = items
                .Where(x => x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        items = sortBy switch
        {
            "name" => sortAsc
                ? items.OrderBy(x => x.Name).ToList()
                : items.OrderByDescending(x => x.Name).ToList(),


            _ => items
        };

        return items;
    }

}
