using EfficiencyTrack.Data.Models;

public interface ICrudService<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid id);

    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm = null,
        string? sortBy = null,
        bool sortAsc = true,
        int pageNumber = 1,
        int pageSize = 10);
}
