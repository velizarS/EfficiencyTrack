using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.EntryViewModel;
using EfficiencyTrack.ViewModels.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EfficiencyTrack.Web.Controllers
{
    public class EntryController : BaseCrudController<
        Entry,
        EntryViewModel,
        EntryListViewModel,
        EntryCreateViewModel,
        EntryEditViewModel,
        EntryDetailsViewModel>
    {
        private readonly IEntryService _entryService;
        private readonly IEmployeeService _employeeService;
        private readonly IRoutingService _routingService;
        private readonly ICrudService<Shift> _shiftService;


        public EntryController(IEntryService entryService, IEmployeeService employeeService, IRoutingService routingService, ICrudService<Shift> shiftService) : base(entryService)
        {
            _entryService = entryService;
            _employeeService = employeeService;
            _routingService = routingService;
            _shiftService = shiftService;
        }

        protected override EntryViewModel MapToViewModel(Entry entity)
        {
            return new EntryViewModel
            {
                Id = entity.Id,
                Date = entity.Date,
                EmployeeId = entity.EmployeeId,
                EmployeeCode = entity.Employee?.Code ?? "N/A",
                EmployeeName = (entity.Employee?.FirstName ?? "") + " " + (entity.Employee?.LastName ?? ""),
                RoutingId = entity.RoutingId,
                RoutingName = entity.Routing?.Code ?? "N/A",
                EfficiencyForOperation = entity.EfficiencyForOperation
            };
        }

        protected override EntryDetailsViewModel MapToDetailModel(Entry entity)
            => new()
            {
                Id = entity.Id,
                Date = entity.Date,
                EmployeeId = entity.EmployeeId,
                EmployeeCode = entity.Employee?.Code ?? "N/A",
                EmployeeName = (entity.Employee?.FirstName ?? "") + " " + (entity.Employee?.LastName ?? ""),
                ShiftId = entity.ShiftId,
                RoutingId = entity.RoutingId,
                RoutingName = entity.Routing?.Code ?? "N/A",
                Pieces = entity.Pieces,
                Scrap = entity.Scrap,
                WorkedMinutes = entity.WorkedMinutes,
                RequiredMinutes = entity.RequiredMinutes,
                EfficiencyForOperation = entity.EfficiencyForOperation
            };

        protected override Entry MapToEntity(EntryCreateViewModel model)
            => new()
            {
                Id = Guid.NewGuid(),
                Date = DateTime.UtcNow,
                ShiftId = model.ShiftId,
                Pieces = model.Pieces,
                Scrap = model.Scrap,
                WorkedMinutes = model.WorkedMinutes,
            };

        protected override Entry MapToEntity(EntryEditViewModel model)
            => new()
            {
                Id = model.Id,
                EmployeeId = model.EmployeeId,
                RoutingId = model.RoutingId,
                ShiftId = model.ShiftId,
                Pieces = model.Pieces,
                Scrap = model.Scrap,
                WorkedMinutes = model.WorkedMinutes,
            };

        protected override EntryEditViewModel MapToEditModel(Entry entity)
            => new()
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                RoutingId = entity.RoutingId,
                ShiftId = entity.ShiftId,
                Pieces = entity.Pieces,
                Scrap = entity.Scrap,
                WorkedMinutes = entity.WorkedMinutes
            };

        protected override EntryListViewModel BuildListViewModel(List<EntryViewModel> items)
            => new() { Entries = items };

        protected override List<EntryViewModel> FilterAndSort(List<EntryViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
        {
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                items = items
                    .Where(x => x.EmployeeCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            items = sortBy switch
            {
                "date" => sortAsc
                    ? items.OrderBy(x => x.Date).ToList()
                    : items.OrderByDescending(x => x.Date).ToList(),

                "employeeName" => sortAsc
                    ? items.OrderBy(x => x.EmployeeName).ToList()
                    : items.OrderByDescending(x => x.EmployeeName).ToList(),

                "routingName" => sortAsc
               ? items.OrderBy(x => x.RoutingName).ToList()
               : items.OrderByDescending(x => x.RoutingName).ToList(),

                "efficiency" => sortAsc
                    ? items.OrderBy(x => x.EfficiencyForOperation).ToList()
                    : items.OrderByDescending(x => x.EfficiencyForOperation).ToList(),

                _ => items
            };

            return items;
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Shifts = await GetShiftSelectListAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Create(EntryCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Shifts = await GetShiftSelectListAsync();
                return View(model);
            }

            var employee = await _employeeService.GetByCodeAsync(model.EmployeeCode);
            if (employee == null)
            {
                ModelState.AddModelError(nameof(model.EmployeeCode), "Няма служител с този код.");
                ViewBag.Shifts = await GetShiftSelectListAsync();
                return View(model);
            }

            var routing = await _routingService.GetRoutingByCodeAsync(model.RoutingCode);
            if (routing == null)
            {
                ModelState.AddModelError(nameof(model.RoutingCode), "Няма операция с този код.");
                ViewBag.Shifts = await GetShiftSelectListAsync();
                return View(model);
            }

            var entry = MapToEntity(model);
            entry.EmployeeId = employee.Id;
            entry.RoutingId = routing.Id;

            await _entryService.AddAsync(entry);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Edit(EntryEditViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingEntry = await _entryService.GetByIdWithIncludesAsync(model.Id);
            if (existingEntry == null)
                return NotFound();

            existingEntry.EmployeeId = model.EmployeeId;
            existingEntry.RoutingId = model.RoutingId;
            existingEntry.ShiftId = model.ShiftId;
            existingEntry.Pieces = model.Pieces;
            existingEntry.Scrap = model.Scrap;
            existingEntry.WorkedMinutes = model.WorkedMinutes;

            await _entryService.UpdateAsync(existingEntry);
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public override async Task<IActionResult> Delete(Guid id)
        {
            await _entryService.DeleteAsync(id); 
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<SelectListItem>> GetShiftSelectListAsync()
        {
            var shifts = await _shiftService.GetAllAsync();

            return shifts.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.Id.ToString()
            }).ToList();
        }
    }
}
