using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.RoutingViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Controllers
{
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

        protected override RoutingViewModel MapToViewModel(Routing entity)
        {
            return new()
            {
                Id = entity.Id,
                Code = entity.Code,
                MinutesPerPiece = entity.MinutesPerPiece,
                DepartmentName = entity.Department?.Name ?? "—",
                Zone = entity.Zone
            };
        }

        protected override RoutingDetailViewModel MapToDetailModel(Routing entity)
        {
            return new()
            {
                Id = entity.Id,
                Code = entity.Code,
                Description = entity.Description,
                Zone = entity.Zone,
                MinutesPerPiece = entity.MinutesPerPiece,
                DepartmentId = entity.DepartmentId,
                DepartmentName = entity.Department?.Name ?? "—"
            };
        }

        protected override Routing MapToEntity(RoutingCreateViewModel model)
        {
            return new()
            {
                Code = model.Code,
                Description = model.Description,
                Zone = model.Zone,
                MinutesPerPiece = model.MinutesPerPiece,
                DepartmentId = model.DepartmentId
            };
        }

        protected override Routing MapToEntity(RoutingEditViewModel model)
        {
            return new()
            {
                Id = model.Id,
                Code = model.Code,
                Description = model.Description,
                Zone = model.Zone,
                MinutesPerPiece = model.MinutesPerPiece,
                DepartmentId = model.DepartmentId
            };
        }

        protected override RoutingEditViewModel MapToEditModel(Routing entity)
        {
            return new()
            {
                Id = entity.Id,
                Code = entity.Code,
                Description = entity.Description,
                Zone = entity.Zone,
                MinutesPerPiece = entity.MinutesPerPiece,
                DepartmentId = entity.DepartmentId
            };
        }

        protected override RoutingListViewModel BuildListViewModel(List<RoutingViewModel> items)
        {
            return new()
            {
                Routings = items
            };
        }

        protected async Task PrepareDropdownsAsync(object? model = null)
        {
            IEnumerable<Department> departments = await _departmentService.GetAllAsync();

            List<SelectListItem> departmentItems = departments
                .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
                .ToList();

            List<SelectListItem> zoneItems =
            [
                new("Zone A", "Zone A"),
                new("Zone B", "Zone B"),
                new("Zone C", "Zone C")
            ];

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

        public override async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true, int page = 1, int pageSize = 20)
        {
            IQueryable<Routing> query = _routingService.GetFilteredRoutings(searchTerm, sortBy, sortAsc);
            List<Routing> entities = await query.AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            List<RoutingViewModel> viewModels = entities.Select(MapToViewModel).ToList();
            RoutingListViewModel listModel = BuildListViewModel(viewModels);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAsc = sortAsc;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(listModel);
        }

        public override async Task<IActionResult> Details(Guid id)
        {
            Routing? entity = await _routingService.GetByIdWithDepartmentAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            RoutingDetailViewModel model = MapToDetailModel(entity);
            return View(model);
        }

        public override async Task<IActionResult> Edit(Guid id)
        {
            Routing? entity = await _routingService.GetByIdWithDepartmentAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            RoutingEditViewModel model = MapToEditModel(entity);
            await PrepareDropdownsAsync(model);
            return View(model);
        }

        public override async Task<IActionResult> Create()
        {
            RoutingCreateViewModel model = new();
            await PrepareDropdownsAsync(model);
            return View(model);
        }
    }
}
