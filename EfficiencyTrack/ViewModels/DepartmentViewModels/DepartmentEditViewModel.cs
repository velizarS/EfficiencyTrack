using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.DepartmentViewModels
{
    public class DepartmentEditViewModel : DepartmentBaseViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
