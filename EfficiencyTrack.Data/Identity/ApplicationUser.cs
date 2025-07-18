using EfficiencyTrack.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace EfficiencyTrack.Data.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            Roles = new HashSet<IdentityUserRole<Guid>>();
            Claims = new HashSet<IdentityUserClaim<Guid>>();
            Logins = new HashSet<IdentityUserLogin<Guid>>();
            Employees = new HashSet<Employee>();
        }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedOn { get; set; }

        public virtual ICollection<IdentityUserRole<Guid>> Roles { get; set; }
        public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }

        public virtual ICollection<Employee> Employees { get; set; }
    }
}
