using EfficiencyTrack.Data.Identity;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.EmployeeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
    private readonly UserManager<ApplicationUser> _userManager;

    public EmployeesController(IEmployeeService employeeService, ICrudService<Department> departmentService, UserManager<ApplicationUser> userManager) : base(employeeService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _userManager = userManager;
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

    private void MapToEntity(EmployeeEditViewModel model, Employee entity)
    {
        entity.Code = model.Code;
        entity.FirstName = model.FirstName;
        entity.MiddleName = model.MiddleName;
        entity.LastName = model.LastName;
        entity.DepartmentId = model.DepartmentId;
        entity.ShiftManagerUserId = model.ShiftManagerUserId;
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

    public override async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true, int page = 1, int pageSize = 20)
    {
        IQueryable<Employee> query;

        if (User.IsInRole("Admin") || User.IsInRole("Manager"))
        {
            query = _employeeService.GetFilteredEmployees(searchTerm, sortBy, sortAsc);
        }
        else if (User.IsInRole("ShiftLeader"))
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                query = Enumerable.Empty<Employee>().AsQueryable();
            }
            else
            {
                query = _employeeService.GetFilteredEmployees(searchTerm, sortBy, sortAsc)
                    .Where(e => e.ShiftManagerUserId == user.Id);
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var employee = await _employeeService.GetByCodeAsync(searchTerm);
                if (employee != null && !employee.IsDeleted)
                {
                    query = new List<Employee> { employee }.AsQueryable();
                }
                else
                {
                    query = Enumerable.Empty<Employee>().AsQueryable();
                    ViewBag.Warning = "Няма служител с въведения код.";
                }
            }
            else
            {
                query = Enumerable.Empty<Employee>().AsQueryable();
                ViewBag.Warning = "Моля, въведете служебния си код за достъп.";
            }
        }

        int totalCount = await query.CountAsync();

        var employees = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();

        var viewModels = employees.Select(emp => new EmployeeViewModel
        {
            Id = emp.Id,
            Code = emp.Code,
            FullName = $"{emp.FirstName} {emp.MiddleName} {emp.LastName}".Replace("  ", " ").Trim(),
            DepartmentName = emp.Department?.Name ?? "-",
            ShiftManagerUserName = emp.ShiftManagerUser?.UserName ?? "-"
        }).ToList();

        var listModel = new EmployeeListViewModel
        {
            Employees = viewModels,
            CurrentPage = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };

        ViewBag.SearchTerm = searchTerm;
        ViewBag.SortBy = sortBy;
        ViewBag.SortAsc = sortAsc;

        return View(listModel);
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

        MapToEntity(model, existing);

        try
        {
            await _employeeService.UpdateAsync(existing);
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
