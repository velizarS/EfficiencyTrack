using EfficiencyTrack.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace EfficiencyTrack.Controllers
{
    public abstract class BaseCrudController<T, TViewModel, TListViewModel, TCreateModel, TEditModel, TDetailModel>
        : Controller where T : BaseEntity
    {
        protected readonly ICrudService<T> _service;

        protected BaseCrudController(ICrudService<T> service)
        {
            _service = service;
        }

        protected virtual List<TViewModel> FilterAndSort(List<TViewModel> items, string? searchTerm, string? sortBy, bool sortAsc)
        {
            return items;
        }

        protected virtual async Task<(List<TViewModel> Items, int TotalCount)> GetPagedAsync(
            string? searchTerm, string? sortBy, bool sortAsc, int page = 1, int pageSize = 20)
        {
            IEnumerable<T> allItems = await _service.GetAllAsync();
            List<TViewModel> vmItems = allItems.Select(MapToViewModel).ToList();

            List<TViewModel> filteredSorted = FilterAndSort(vmItems, searchTerm, sortBy, sortAsc);

            return (filteredSorted, filteredSorted.Count);
        }

        public virtual async Task<IActionResult> Index(string? searchTerm, string? sortBy, bool sortAsc = true, int page = 1, int pageSize = 20)
        {
            (List<TViewModel> items, int totalCount) = await GetPagedAsync(searchTerm, sortBy, sortAsc, page, pageSize);

            TListViewModel? listViewModel = BuildListViewModel(items);

            ViewBag.SearchTerm = searchTerm;
            ViewBag.SortBy = sortBy;
            ViewBag.SortAsc = sortAsc;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = totalCount;

            return View(listViewModel);
        }

        public virtual async Task<IActionResult> Details(Guid id)
        {
            T? entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            TDetailModel? viewModel = MapToDetailModel(entity);
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

            T entity = MapToEntity(model);
            await _service.AddAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public virtual async Task<IActionResult> Edit(Guid id)
        {
            T? entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            TEditModel? viewModel = MapToEditModel(entity);
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

            T entity = MapToEntity(model);
            await _service.UpdateAsync(entity);
            return RedirectToAction(nameof(Index));
        }

        public virtual async Task<IActionResult> Delete(Guid id)
        {
            T? entity = await _service.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            TDetailModel? viewModel = MapToDetailModel(entity);
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(Guid id)
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
}
