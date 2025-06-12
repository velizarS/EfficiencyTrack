using System;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.Employee
{
    public class EmployeeCreateViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "Code cannot be longer than 20 characters.")]
        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Code must consist of uppercase letters and numbers only.")]
        [Display(Name = "Код на служител")]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[\p{L}\s\-']+$", ErrorMessage = "Name must consist of letters, spaces, hyphens or apostrophes.")]
        [Display(Name = "Име")]
        public string FirstName { get; set; } = null!;

        [StringLength(100)]
        [RegularExpression(@"^[\p{L}\s\-']+$", ErrorMessage = "Name must consist of letters, spaces, hyphens or apostrophes.")]
        [Display(Name = "Презиме")]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        [RegularExpression(@"^[\p{L}\s\-']+$", ErrorMessage = "Name must consist of letters, spaces, hyphens or apostrophes.")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Началник смяна")]
        public Guid? ShiftManagerUserId { get; set; }


        [Required]
        [Display(Name = "Отдел")]
        public Guid DepartmentId { get; set; }
    }
}
