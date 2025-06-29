using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Implementations
{
    public class DepartmentService : CrudService<Department>, IDepartmentService
    {
        private readonly EfficiencyTrackDbContext _context;

        public DepartmentService(
            EfficiencyTrackDbContext context,
            IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            _context = context;
        }

        public async Task<Department?> GetDepartmentWithEmployeesAsync(Guid id)
        {
            return await _context.Departments
                                 .Include(d => d.Employees.Where(e => !e.IsDeleted))
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted);
        }

    }
}
