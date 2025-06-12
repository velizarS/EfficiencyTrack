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

        public List<string> EmployeeNames { get; set; } = new();
    }
}
