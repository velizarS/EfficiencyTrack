using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.DepartmentViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyTrack.Controllers;

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
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
        };
    }

    protected override DepartmentDetailViewModel MapToDetailModel(Department entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Employees = entity.Employees
                    .Where(e => !e.IsDeleted)
                    .Select(e => new EmployeeSimpleViewModel
                    {
                        Code = e.Code,
                        FullName = $"{e.FirstName} {e.LastName}"
                    })
                    .ToList()
        };
    }

    protected override Department MapToEntity(DepartmentCreateViewModel model)
    {
        return new()
        {
            Name = model.Name
        };
    }

    protected override Department MapToEntity(DepartmentEditViewModel model)
    {
        return new()
        {
            Id = model.Id,
            Name = model.Name
        };
    }

    protected override DepartmentEditViewModel MapToEditModel(Department entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name
        };
    }

    protected override DepartmentListViewModel BuildListViewModel(List<DepartmentViewModel> items)
    {
        return new()
        {
            Departments = items
        };
    }

    protected override List<DepartmentViewModel> FilterAndSort(List<DepartmentViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            items = items
                .Where(x => x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        items = sortBy?.ToLower() switch
        {
            "name" => sortAsc
                ? items.OrderBy(x => x.Name).ToList()
                : items.OrderByDescending(x => x.Name).ToList(),
            _ => items
        };

        return items;
    }

    public override async Task<IActionResult> Details(Guid id)
    {
        Department? departmentWithEmployees = await _departmentService.GetDepartmentWithEmployeesAsync(id);

        if (departmentWithEmployees == null)
        {
            return NotFound();
        }

        DepartmentDetailViewModel viewModel = MapToDetailModel(departmentWithEmployees);

        return View(viewModel);
    }
}
