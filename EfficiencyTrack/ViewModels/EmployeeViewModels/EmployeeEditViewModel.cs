using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EmployeeViewModels
{
    public class EmployeeEditViewModel : EmployeeBaseViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}
