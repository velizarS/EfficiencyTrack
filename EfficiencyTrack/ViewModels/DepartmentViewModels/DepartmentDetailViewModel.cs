using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using EfficiencyTrack.ViewModels;

namespace EfficiencyTrack.ViewModels.Department
{
    public class DepartmentDetailViewModel : BaseDetailViewModel
    {
        [Display(Name = "Име на отдел")]
        public string Name { get; set; } = null!;

        public List<EmployeeSimpleViewModel> Employees { get; set; } = new();
    }

    public class EmployeeSimpleViewModel
    {
        public string Code { get; set; } = null!;
        public string FullName { get; set; } = null!;
    }
}
