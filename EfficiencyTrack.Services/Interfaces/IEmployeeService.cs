using EfficiencyTrack.Data.Models;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IEmployeeService : ICrudService<Employee>
    {
        Task<List<Employee>> GetAllActiveAsync();
        Task<List<Employee>> GetByDepartmentAsync(Guid departmentId);
        Task<List<Employee>> GetByLeaderIdAsync(Guid leaderId);
        Task<Employee?> GetByApplicationUserIdAsync(Guid userId);
        Task<bool> IsEmployeeCodeUniqueAsync(string code, Guid? excludeId = null);
        Task DeactivateAsync(Guid employeeId);
    }
}
