using EfficiencyTrack.Data.Models;

public interface ICrudService<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);

    Task<(IEnumerable<T> Items, int TotalCount)> GetPagedAsync(
        string? searchTerm,
        string? sortBy,
        bool sortAsc,
        int pageNumber,
        int pageSize);
}
