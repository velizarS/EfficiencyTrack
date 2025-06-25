using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyTrack.Web.Controllers;

public abstract class BaseCrudController<T, TViewModel, TListViewModel, TCreateModel, TEditModel, TDetailModel>
    : Controller where T : BaseEntity
{
    protected readonly ICrudService<T> _service;

    protected BaseCrudController(ICrudService<T> service)
    {
        _service = service;
    }

    public virtual async Task<IActionResult> Index()
    {
        var items = await _service.GetAllAsync();
        var viewModels = items.Select(MapToViewModel).ToList();
        var listViewModel = BuildListViewModel(viewModels);
        return View(listViewModel);
    }

    public virtual async Task<IActionResult> Details(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();

        var viewModel = MapToDetailModel(item);
        return View(viewModel);
    }

    public virtual async Task<IActionResult> Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Create(TCreateModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = MapToEntity(model);
        await _service.AddAsync(entity);
        return RedirectToAction(nameof(Index));
    }

    public virtual async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();

        var viewModel = MapToEditModel(entity);
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual async Task<IActionResult> Edit(TEditModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var entity = MapToEntity(model);
        await _service.UpdateAsync(entity);
        return RedirectToAction(nameof(Index));
    }

    public virtual async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    protected abstract TViewModel MapToViewModel(T entity);
    protected abstract TDetailModel MapToDetailModel(T entity);
    protected abstract T MapToEntity(TCreateModel model);
    protected abstract T MapToEntity(TEditModel model);
    protected abstract TEditModel MapToEditModel(T entity);
    protected abstract TListViewModel BuildListViewModel(List<TViewModel> items);

}
