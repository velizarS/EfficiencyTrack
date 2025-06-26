using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.EntryViewModel;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyTrack.Web.Controllers;

public class EntryController : BaseCrudController<
    Entry,
    EntryViewModel,
    EntryListViewModel,
    EntryCreateViewModel,
    EntryEditViewModel,
    EntryDetailsViewModel>
{
    private readonly IEntryService _entryService;

    public EntryController(IEntryService entryService) : base(entryService)
    {
        _entryService = entryService;
    }

    protected override EntryViewModel MapToViewModel(Entry entity)
        => new()
        {
            Date = entity.Date,
            EmployeeId = entity.EmployeeId,
            RoutingId = entity.RoutingId,
            EfficiencyForOperation = entity.EfficiencyForOperation
        };

    protected override EntryDetailsViewModel MapToDetailModel(Entry entity)
        => new()
        {
            Id = entity.Id,
            Date = entity.Date,
            EmployeeId = entity.EmployeeId,
            ShiftId = entity.ShiftId,
            RoutingId = entity.RoutingId,
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
            EmployeeId = model.EmployeeId,
            RoutingId = model.RoutingId,
            ShiftId = model.ShiftId,
            Pieces = model.Pieces,
            Scrap = model.Scrap,
            WorkedMinutes = model.WorkedMinutes,
            // НЕ добавяме RequiredMinutes тук, то се изчислява в сервиса
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
            // НЕ добавяме RequiredMinutes тук, то се изчислява в сервиса
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
            // НЕ добавяме RequiredMinutes тук, защото не се променя директно
        };

    protected override EntryListViewModel BuildListViewModel(List<EntryViewModel> items)
        => new() { Entries = items };

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create(EntryCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var entry = MapToEntity(model);
        await _entryService.SetEfficiencyAsync(entry);

        await _service.AddAsync(entry);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(EntryEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var entry = MapToEntity(model);
        await _entryService.SetEfficiencyAsync(entry);

        await _service.UpdateAsync(entry);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
