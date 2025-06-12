namespace EfficiencyTrack.ViewModels.DepartmentViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    namespace EfficiencyTrack.ViewModels.Department
    {
        public class DepartmentViewModel
        {
            public Guid Id { get; set; }

            [Display(Name = "Име на отдел")]
            public string Name { get; set; } = null!;
        }
    }


}
