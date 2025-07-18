using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EmployeeViewModels
{
    public abstract class EmployeeDisplayBaseViewModel
    {
        [Display(Name = "Код на служител")]
        public string Code { get; set; } = null!;

        [Display(Name = "Име")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Отдел")]
        public string DepartmentName { get; set; } = null!;

        [Display(Name = "Началник смяна")]
        public string? ShiftManagerUserName { get; set; }
    }
}
