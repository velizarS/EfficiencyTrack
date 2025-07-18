﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfficiencyTrack.Data.Models
{
    [Comment("Represents the daily efficiency of an employee, calculated automatically based on performed tasks and shift time.")]
    public class DailyEfficiency : BaseEntity
    {


        [Display(Name = "Date")]
        [Comment("The date for which this daily efficiency entry is calculated.")]
        public DateTime Date { get; set; }

        [ForeignKey(nameof(Employee))]
        [Display(Name = "Employee")]
        [Comment("The employee this daily efficiency record belongs to.")]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        [ForeignKey(nameof(Shift))]
        [Display(Name = "Shift")]
        [Comment("The shift during which the employee worked.")]
        public Guid ShiftId { get; set; }
        public Shift Shift { get; set; } = null!;


        [Display(Name = "Total Worked Minutes")]
        [Comment("Total actual time worked by the employee on that day.")]
        [Column(TypeName = "decimal(10,4)")]
        public decimal TotalWorkedMinutes { get; set; }

        [Display(Name = "Total needed Minutes")]
        [Comment("Total needed time .")]
        [Column(TypeName = "decimal(10,4)")]
        public decimal TotalNeededMinutes { get; set; }


        [Display(Name = "Efficiency (%)")]
        [Comment("Calculated efficiency as (total needed minutes based on operations / shift time) * 100.")]
        [Column(TypeName = "decimal(10,4)")]
        public decimal EfficiencyPercentage { get; set; }

        [Display(Name = "Calculated At")]
        [Comment("Timestamp when the efficiency was last calculated.")]
        public DateTime ComputedOn { get; set; }
    }
}
