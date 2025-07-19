using EfficiencyTrack.Controllers;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.ViewModels.ShiftViewModels;
using Microsoft.AspNetCore.Authorization;

[Authorize]
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
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            DurationMinutes = entity.DurationMinutes
        };
    }

    protected override ShiftDetailViewModel MapToDetailModel(Shift entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            DurationMinutes = entity.DurationMinutes
        };
    }

    protected override Shift MapToEntity(ShiftCreateViewModel model)
    {
        return new()
        {
            Name = model.Name,
            DurationMinutes = model.DurationMinutes
        };
    }

    protected override Shift MapToEntity(ShiftEditViewModel model)
    {
        return new()
        {
            Id = model.Id,
            Name = model.Name,
            DurationMinutes = model.DurationMinutes
        };
    }

    protected override ShiftEditViewModel MapToEditModel(Shift entity)
    {
        return new()
        {
            Id = entity.Id,
            Name = entity.Name,
            DurationMinutes = entity.DurationMinutes
        };
    }

    protected override ShiftListViewModel BuildListViewModel(List<ShiftViewModel> items)
    {
        return new()
        {
            Shifts = items
        };
    }

    protected override List<ShiftViewModel> FilterAndSort(List<ShiftViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
    {
        IEnumerable<ShiftViewModel> query = items;

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x => x.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        query = sortBy switch
        {
            "name" => sortAsc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
            "duration" => sortAsc ? query.OrderBy(x => x.DurationMinutes) : query.OrderByDescending(x => x.DurationMinutes),
            _ => query
        };

        return query.ToList();
    }

}
