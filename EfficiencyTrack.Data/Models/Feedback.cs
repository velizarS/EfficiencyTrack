using EfficiencyTrack.Data.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Comment("Represents user feedback in the system.")]
public class Feedback
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey(nameof(User))]
    [Display(Name = "User")]
    [Comment("The user who provided the feedback.")]
    public Guid? UserId { get; set; }

    [StringLength(20, ErrorMessage = "Code cannot be longer than 20 characters.")]
    [Display(Name = "Employee Code")]
    [Comment("The unique code for the employee.")]
    [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code must consist of uppercase letters and numbers only.")]
    public string? EmployeeCode { get; set; }

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

    public ApplicationUser? User { get; set; }
}
