using System;
using System.ComponentModel.DataAnnotations;
using EfficiencyTrack.ViewModels;

namespace EfficiencyTrack.ViewModels.Employee
{
    public class EmployeeDetailViewModel : BaseDetailViewModel
    {
        [Display(Name = "Код на служител")]
        public string Code { get; set; } = null!;

        [Display(Name = "Име")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Презиме")]
        public string? MiddleName { get; set; }

        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = null!;

        public Guid? ShiftManagerUserId { get; set; }

        [Display(Name = "Началник смяна")]
        public string? ShiftManagerUserName { get; set; }

        public Guid DepartmentId { get; set; }

        [Display(Name = "Отдел")]
        public string DepartmentName { get; set; } = null!;
    }

}
