using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyTrack.Data.Models
{
    public class ApllicationUser : IdentityUser<Guid>
    {
        public ApllicationUser()
        {
            Id = Guid.NewGuid();
        }
    }
}
