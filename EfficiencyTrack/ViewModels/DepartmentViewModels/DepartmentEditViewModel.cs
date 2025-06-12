using System;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.Department
{
    public class DepartmentEditViewModel
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        [Display(Name = "Име на отдел")]
        public string Name { get; set; } = null!;
    }
}
