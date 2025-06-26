using EfficiencyTrack.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IDepartmentService : ICrudService<Department>
    {
        Task<Department?> GetDepartmentWithEmployeesAsync(Guid id);
    }

}
