using EfficiencyTrack.Data.Models;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IEmployeeService : ICrudService<Employee>
    {
        Task<List<Employee>> GetAllActiveAsync();
        Task<List<Employee>> GetByDepartmentAsync(Guid departmentId);
        Task<List<Employee>> GetByShiftManagerUserIdAsync(Guid leaderId);
        Task<bool> IsEmployeeCodeUniqueAsync(string code, Guid? excludeId = null);
        Task DeactivateAsync(Guid employeeId);
    }
}
