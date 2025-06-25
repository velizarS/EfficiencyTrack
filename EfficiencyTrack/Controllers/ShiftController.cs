using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using EfficiencyTrack.ViewModels.Shift;
using EfficiencyTrack.Web.Controllers;

public class ShiftController : BaseCrudController<
    Shift,
    ShiftViewModel,
    ShiftListViewModel,
    ShiftCreateViewModel,
    ShiftEditViewModel,
    ShiftDetailViewModel>
{
    public ShiftController(ICrudService<Shift> service)
        : base(service) { }

    protected override ShiftViewModel MapToViewModel(Shift entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            DurationMinutes = entity.DurationMinutes
        };

    protected override ShiftDetailViewModel MapToDetailModel(Shift entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            DurationMinutes = entity.DurationMinutes
        };                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                     

    protected override Shift MapToEntity(ShiftCreateViewModel model)
        => new()
        {
            Name = model.Name,
            DurationMinutes = model.DurationMinutes
        };

    protected override Shift MapToEntity(ShiftEditViewModel model)
        => new()
        {
            Id = model.Id,
            Name = model.Name,
            DurationMinutes = model.DurationMinutes
        };

    protected override ShiftEditViewModel MapToEditModel(Shift entity)
        => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            DurationMinutes = entity.DurationMinutes
        };

    protected override ShiftListViewModel BuildListViewModel(List<ShiftViewModel> items)
        => new()
        {
            Shifts = items
        };
}
