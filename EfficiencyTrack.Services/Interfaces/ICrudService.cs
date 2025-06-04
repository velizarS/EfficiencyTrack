using EfficiencyTrack.Data.Models;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface ICrudService<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
    }

}
