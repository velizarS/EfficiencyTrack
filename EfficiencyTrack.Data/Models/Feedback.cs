using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.Data.Models
{
    [Comment("Represents user feedback in the system.")]
    [Index(nameof(IsHandled))]
    public class Feedback
    {
        [Key]
        public Guid Id { get; set; }

        [StringLength(100, ErrorMessage = "Employee name cannot be longer than 100 characters.")]
        [Display(Name = "Employee Name")]
        [Comment("The full name of the employee providing the feedback.")]
        public string? EmployeeName { get; set; }

        [Required(ErrorMessage = "Message is required.")]
        [StringLength(4000, ErrorMessage = "Message cannot be longer than 4000 characters.")]
        [Display(Name = "Feedback Message")]
        [Comment("The feedback message provided by the user.")]
        public string Message { get; set; } = null!;

        [Required]
        [Display(Name = "Created At")]
        [Comment("The date and time when the feedback was created.")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Display(Name = "Handled")]
        [Comment("Indicates whether the feedback has been handled.")]
        public bool IsHandled { get; set; } = false;

        [Display(Name = "Handled At")]
        [Comment("The date and time when the feedback was handled.")]
        public DateTime? HandledAt { get; set; }
    }
}