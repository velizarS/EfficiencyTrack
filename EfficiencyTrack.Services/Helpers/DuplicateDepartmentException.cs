using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EfficiencyTrack.Services.Helpers
{
    public class DuplicateDepartmentException : Exception
    {
        public DuplicateDepartmentException(string message) : base(message) { }
    }

}
