using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.Employee;
using EfficiencyTrack.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyTrack.Web.Controllers;

public class EmployeesController : BaseCrudController<
    Employee,
    EmployeeViewModel,
    EmployeeListViewModel,
    EmployeeCreateViewModel,
    EmployeeEditViewModel,
    EmployeeDetailViewModel>
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService) : base(employeeService)
    {
        _employeeService = employeeService;
    }

    protected override EmployeeViewModel MapToViewModel(Employee e)
        => new()
        {
            Code = e.Code,
            FullName = $"{e.FirstName} {e.MiddleName} {e.LastName}".Replace("  ", " ").Trim(),
            DepartmentName = e.Department?.Name ?? "(няма отдел)",
            IsActive = e.IsActive
        };

    protected override EmployeeDetailViewModel MapToDetailModel(Employee e)
        => new()
        {
            Id = e.Id,
            Code = e.Code,
            FirstName = e.FirstName,
            MiddleName = e.MiddleName,
            LastName = e.LastName,
            IsActive = e.IsActive,
            ShiftManagerUserId = e.ShiftManagerUserId,
            ShiftManagerUserName = e.ShiftManagerUser?.UserName,
            DepartmentId = e.DepartmentId,
            DepartmentName = e.Department?.Name ?? "(няма отдел)"
        };

    protected override Employee MapToEntity(EmployeeCreateViewModel model)
        => new()
        {
            Id = Guid.NewGuid(),
            Code = model.Code,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            IsActive = true,
            DepartmentId = model.DepartmentId,
            ShiftManagerUserId = model.ShiftManagerUserId
        };

    protected override Employee MapToEntity(EmployeeEditViewModel model)
        => new()
        {
            Id = model.Id,
            Code = model.Code,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            IsActive = model.IsActive,
            DepartmentId = model.DepartmentId,
            ShiftManagerUserId = model.ShiftManagerUserId
        };

    protected override EmployeeEditViewModel MapToEditModel(Employee e)
        => new()
        {
            Id = e.Id,
            Code = e.Code,
            FirstName = e.FirstName,
            MiddleName = e.MiddleName,
            LastName = e.LastName,
            IsActive = e.IsActive,
            DepartmentId = e.DepartmentId,
            ShiftManagerUserId = e.ShiftManagerUserId
        };

    protected override EmployeeListViewModel BuildListViewModel(List<EmployeeViewModel> employees)
        => new() { Employees = employees };

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create(EmployeeCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!await _employeeService.IsEmployeeCodeUniqueAsync(model.Code))
        {
            ModelState.AddModelError(nameof(model.Code), "Кодът вече съществува.");
            return View(model);
        }

        var entity = MapToEntity(model);
        await _service.AddAsync(entity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(EmployeeEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!await _employeeService.IsEmployeeCodeUniqueAsync(model.Code, model.Id))
        {
            ModelState.AddModelError(nameof(model.Code), "Кодът вече съществува.");
            return View(model);
        }

        var entity = MapToEntity(model);
        await _service.UpdateAsync(entity);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        await _employeeService.DeactivateAsync(id);
        return RedirectToAction(nameof(Index));
    }

}
