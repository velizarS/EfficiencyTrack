using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.Data.Models
{
    [Comment("Represents a department within the organization.")]
    public class Department : BaseEntity
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "Department Name")]
        [Comment("The name of the department.")]
        public string Name { get; set; } = null!;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
