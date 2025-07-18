using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.DepartmentViewModels
{
    public class DepartmentBaseViewModel
    {
        [Required]
        [StringLength(50, ErrorMessage = "Името не може да е по-дълго от 50 символа.")]
        [Display(Name = "Име на отдел")]
        public string Name { get; set; } = null!;
    }
}
