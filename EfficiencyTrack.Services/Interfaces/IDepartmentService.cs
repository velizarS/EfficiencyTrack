using EfficiencyTrack.Services.DTOs;
using EfficiencyTrack.Data.Models;
using System;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IDepartmentService : ICrudService<Department>
    {
        Task<Department?> GetDepartmentWithEmployeesAsync(Guid id);
    }
}
