using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EmployeeViewModels
{
    public abstract class EmployeeBaseViewModel
    {
        [Required]
        [StringLength(20)]
        [RegularExpression(@"^[A-Z0-9]+$")]
        [Display(Name = "Код на служител")]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\d\s\-']+$")]
        [Display(Name = "Име")]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\d\s\-']+$")]
        [Display(Name = "Презиме")]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[a-zA-Z\d\s\-']+$")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Началник смяна")]
        public Guid? ShiftManagerUserId { get; set; }

        [Required]
        [Display(Name = "Отдел")]
        public Guid DepartmentId { get; set; }
    }
}
