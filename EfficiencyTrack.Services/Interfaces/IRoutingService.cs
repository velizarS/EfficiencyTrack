using EfficiencyTrack.Data.Models;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IRoutingService : ICrudService<Routing>
    {
        Task<Routing> GetRoutingByCodeAsync(string routingCode);
        Task<Routing?> GetByIdWithDepartmentAsync(Guid id);
        Task<IEnumerable<Routing>> GetAllWithDepartmentsAsync();
        IQueryable<Routing> GetFilteredRoutings(string? searchTerm, string? sortBy, bool sortAsc);

    }
}
