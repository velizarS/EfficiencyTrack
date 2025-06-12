using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.Department
{
    public class DepartmentCreateViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        [Display(Name = "Име на отдел")]
        public string Name { get; set; } = null!;
    }
}
