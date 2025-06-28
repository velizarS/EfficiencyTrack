using System;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.Employee
{
    public class EmployeeViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Код на служител")]
        public string Code { get; set; } = null!;

        [Display(Name = "Име")]
        public string FullName { get; set; } = null!;

        [Display(Name = "Отдел")]
        public string DepartmentName { get; set; } = null!;

        [Display(Name = "Ръководител")]
        public string ShiftLeader { get; set; } = null!;

        
    }
}
