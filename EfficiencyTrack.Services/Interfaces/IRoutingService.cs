using EfficiencyTrack.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Interfaces
{
    public interface IRoutingService : ICrudService<Routing>
    {
        Task<Routing> GetRoutingByCodeAsync(string routingCode);

    }
}