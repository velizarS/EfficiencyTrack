using EfficiencyTrack.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.Data.Models
{
    [Comment("Represents an employee in the system.")]
    public class Employee : BaseEntity
    {
        [Required]
        [StringLength(20, ErrorMessage = "Code cannot be longer than 20 characters.")]
        [Display(Name = "Employee Code")]
        [Comment("The unique code for the employee.")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code must consist of uppercase letters and numbers only.")]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(100, ErrorMessage = "First name cannot be longer than 100 characters.")]
        [Display(Name = "First Name")]
        [Comment("The first name of the employee.")]
        [RegularExpression(@"^[\p{L}\s\-']+$", ErrorMessage = "Name must consist of letters, spaces, hyphens or apostrophes.")]
        public string FirstName { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Middle name cannot be longer than 100 characters.")]
        [Display(Name = "Middle Name")]
        [Comment("The middle name of the employee.")]
        [RegularExpression(@"^[\p{L}\s\-']+$", ErrorMessage = "Name must consist of letters, spaces, hyphens or apostrophes.")]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Last name cannot be longer than 100 characters.")]
        [Display(Name = "Last Name")]
        [Comment("The last name of the employee.")]
        [RegularExpression(@"^[\p{L}\s\-']+$", ErrorMessage = "Name must consist of letters, spaces, hyphens or apostrophes.")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Is Active")]
        [Comment("Indicates if the employee is currently active.")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Leader")]
        [Comment("Reference to the employee's leader.")]
        public Guid? LeaderId { get; set; }

        [ForeignKey(nameof(LeaderId))]
        public Employee? Leader { get; set; }

        [InverseProperty(nameof(Leader))]
        [Comment("The collection of employees who report to this employee.")]
        public ICollection<Employee> Team { get; set; } = new List<Employee>();

        [Required]
        [Display(Name = "Department")]
        [Comment("The department to which the employee belongs.")]
        public Guid DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        [InverseProperty(nameof(Entry.Employee))]
        public ICollection<Entry> Entries { get; set; } = new List<Entry>();

    }
}
