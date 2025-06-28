using EfficiencyTrack.Data.Data;
using EfficiencyTrack.Data.Models;
using EfficiencyTrack.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Implementations
{
    public class RoutingService : CrudService<Routing>, IRoutingService
    {
        private readonly EfficiencyTrackDbContext _context;

        public RoutingService(EfficiencyTrackDbContext context, IHttpContextAccessor httpContextAccessor)
            : base(context, httpContextAccessor)
        {
            _context = context;
        }

        public async Task<Routing> GetRoutingByCodeAsync(string routingCode)
        {
            return await _context.Routings
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Code == routingCode && !r.IsDeleted);
        }
    }
}
