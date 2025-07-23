using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EfficiencyTrack.Controllers;

[Authorize]
public class EmployeesController : BaseCrudController<
    Employee,
    EmployeeViewModel,
    EmployeeListViewModel,
    EmployeeCreateViewModel,
    EmployeeEditViewModel,
    EmployeeDetailViewModel>
{
    private readonly IEmployeeService _employeeService;
    private readonly ICrudService<Department> _departmentService;

    public EmployeesController(IEmployeeService employeeService, ICrudService<Department> departmentService) : base(employeeService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
    }

    protected override EmployeeViewModel MapToViewModel(Employee e)
    {
        return new()
        {
            Id = e.Id,
            Code = e.Code,
            FullName = $"{e.FirstName} {e.MiddleName} {e.LastName}".Replace("  ", " ").Trim(),
            DepartmentName = e.Department?.Name ?? "(няма отдел)",
            ShiftManagerUserName = e.ShiftManagerUser?.UserName ?? ""
        };
    }

    protected override EmployeeDetailViewModel MapToDetailModel(Employee e)
    {
        return new()
        {
            Id = e.Id,
            Code = e.Code,
            FullName = $"{e.FirstName} {e.MiddleName} {e.LastName}".Replace("  ", " ").Trim(),
            ShiftManagerUserId = e.ShiftManagerUserId,
            ShiftManagerUserName = e.ShiftManagerUser?.UserName,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name ?? "(няма отдел)"
        };
    }

    protected override Employee MapToEntity(EmployeeCreateViewModel model)
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Code = model.Code,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            DepartmentId = model.DepartmentId,
            ShiftManagerUserId = model.ShiftManagerUserId
        };
    }

    protected override Employee MapToEntity(EmployeeEditViewModel model)
    {
        return new()
        {
            Id = model.Id,
            Code = model.Code,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            DepartmentId = model.DepartmentId,
            ShiftManagerUserId = model.ShiftManagerUserId
        };
    }

    protected override EmployeeEditViewModel MapToEditModel(Employee e)
    {
        return new()
        {
            Id = e.Id,
            Code = e.Code,
            FirstName = e.FirstName,
            MiddleName = e.MiddleName,
            LastName = e.LastName,
            DepartmentId = e.DepartmentId,
            ShiftManagerUserId = e.ShiftManagerUserId
        };
    }

    protected override EmployeeListViewModel BuildListViewModel(List<EmployeeViewModel> employees)
    {
        return new() { Employees = employees };
    }

    [HttpGet]
    public override async Task<IActionResult> Create()
    {
        await LoadSelectLists();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create(EmployeeCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return View(model);
        }

        if (!await _employeeService.IsEmployeeCodeUniqueAsync(model.Code))
        {
            ModelState.AddModelError(nameof(model.Code), "Кодът вече съществува.");
            await LoadSelectLists();
            return View(model);
        }

        Employee entity = MapToEntity(model);

        try
        {
            await _service.AddAsync(entity);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadSelectLists();
            return View(model);
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        Employee? employee = await _employeeService.GetByIdAsync(id);
        if (employee == null)
        {
            return NotFound();
        }

        EmployeeEditViewModel model = MapToEditModel(employee);
        await LoadSelectLists();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(EmployeeEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadSelectLists();
            return View(model);
        }

        if (!await _employeeService.IsEmployeeCodeUniqueAsync(model.Code, model.Id))
        {
            ModelState.AddModelError(nameof(model.Code), "Кодът вече съществува.");
            await LoadSelectLists();
            return View(model);
        }

        Employee? existing = await _employeeService.GetByIdAsync(model.Id);
        if (existing == null)
        {
            return NotFound();
        }

        Employee entity = MapToEntity(model);

        try
        {
            await _service.UpdateAsync(entity);
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await LoadSelectLists();
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    protected override List<EmployeeViewModel> FilterAndSort(List<EmployeeViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            items = items
                .Where(x => x.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                         || x.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        items = sortBy?.ToLower() switch
        {
            "fullname" => sortAsc
                ? items.OrderBy(x => x.FullName).ToList()
                : items.OrderByDescending(x => x.FullName).ToList(),

            "department" => sortAsc
                ? items.OrderBy(x => x.DepartmentName).ToList()
                : items.OrderByDescending(x => x.DepartmentName).ToList(),

            "shiftmanager" => sortAsc
                ? items.OrderBy(x => x.ShiftManagerUserName).ToList()
                : items.OrderByDescending(x => x.ShiftManagerUserName).ToList(),

            "code" => sortAsc
                ? items.OrderBy(x => x.Code).ToList()
                : items.OrderByDescending(x => x.Code).ToList(),

            _ => items
        };

        return items;
    }

    private async Task LoadSelectLists()
    {
        List<Department> departments = (await _departmentService.GetAllAsync()).ToList();
        List<Data.Identity.ApplicationUser> shiftManagers = (await _employeeService.GetAllShiftManagersAsync()).ToList();

        ViewBag.Departments = departments.Select(d => new SelectListItem
        {
            Text = d.Name,
            Value = d.Id.ToString()
        });

        ViewBag.ShiftManagers = shiftManagers.Select(m => new SelectListItem
        {
            Text = m.UserName,
            Value = m.Id.ToString()
        });
    }
}
