using EfficiencyTrack.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [Display(Name = "Shift Manager")]
        [Comment("Reference to the employee's shift manager (ApplicationUser).")]
        public Guid? ShiftManagerUserId { get; set; }

        [ForeignKey(nameof(ShiftManagerUserId))]
        public ApplicationUser? ShiftManagerUser { get; set; }

        [Required]
        [Display(Name = "Department")]
        [Comment("The department to which the employee belongs.")]
        public Guid DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        [InverseProperty(nameof(Entry.Employee))]
        public ICollection<Entry> Entries { get; set; } = new List<Entry>();

        [Display(Name = "Application User")]
        [Comment("Reference to the associated application user.")]
        public Guid? ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
