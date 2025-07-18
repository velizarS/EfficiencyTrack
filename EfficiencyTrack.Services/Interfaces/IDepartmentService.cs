using EfficiencyTrack.Data.Models;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IDepartmentService : ICrudService<Department>
    {
        Task<Department?> GetDepartmentWithEmployeesAsync(Guid id);
    }
}
