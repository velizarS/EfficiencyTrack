using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.RoutingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EfficiencyTrack.Controllers
{
    [Authorize]
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
                new("Assembly", "1"),
                new("AUX", "2"),
                new("CMC-assembling", "3"),
                new("CMC-testing", "4"),
                new("Final-Assembly", "5"),
                new("Finalization", "6"),
                new("Finalization manual table", "7"),
                new("G1", "8"),
                new("G2", "9"),
                new("Kinematik", "10"),
                new("Magnetic", "11"),
                new("Magnetic Epson", "12"),
                new("Main bimetal ", "13"),
                new("Main Epson ", "14"),
                new("Multiple ass", "15"),
                new("Neutrals", "16"),
                new("NMCB Automatic Lines", "17"),
                new("OPF", "18"),
                new("Pre - finalization", "19"),
                new("Pre-Assembly", "20"),
                new("ProM", "21"),
                new("QC", "22"),
                new("QS/T1V", "23"),
                new("Selective", "24"),
                new("Sockets", "25"),
                new("T2", "26"),
                new("Testing", "27"),
                new("Testing line", "28"),
                new("Testing sockets", "29"),
                new("Titan Pos.1", "30"),
                new("Visualization", "31"),
                new("Weldings", "32")
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

            int totalCount = await query.CountAsync();
            List<Routing> entities = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            List<RoutingViewModel> viewModels = entities.Select(MapToViewModel).ToList();

            var listModel = new RoutingListViewModel
            {
                Routings = viewModels,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAsc = sortAsc;

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
