using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.Routing;
using EfficiencyTrack.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfficiencyTrack.Web.Controllers
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
                .Select(d => new SelectListItem(d.Name, d.Id.ToString()))
                .ToList();

            var zoneItems = new List<SelectListItem>
            {
                new("Zone A", "Zone A"),
                new("Zone B", "Zone B"),
                new("Zone C", "Zone C")
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

        public override async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true, int page = 1, int pageSize = 20)
        {
            var query = _routingService.GetFilteredRoutings(searchTerm, sortBy, sortAsc);
            var entities = await query.AsNoTracking().Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var viewModels = entities.Select(MapToViewModel).ToList();
            var listModel = BuildListViewModel(viewModels);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAsc = sortAsc;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(listModel);
        }

        public override async Task<IActionResult> Details(Guid id)
        {
            var entity = await _routingService.GetByIdWithDepartmentAsync(id);
            if (entity == null)
                return NotFound();

            var model = MapToDetailModel(entity);
            return View(model);
        }

        public override async Task<IActionResult> Edit(Guid id)
        {
            var entity = await _routingService.GetByIdWithDepartmentAsync(id);
            if (entity == null)
                return NotFound();

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
    }
}
