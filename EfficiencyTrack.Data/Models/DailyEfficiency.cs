using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.Data.Models
{
    [Comment("Represents the daily efficiency of an employee, calculated based on the tasks performed and time worked.")]
    public class DailyEfficiency : BaseEntity
    {
        [Required]
        [Display(Name = "Date")]
        [Comment("The date for which this daily efficiency entry is calculated.")]
        public DateTime Date { get; set; }

        [Required]
        [ForeignKey(nameof(Employee))]
        [Display(Name = "Employee")]
        [Comment("The employee this daily efficiency record belongs to.")]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total needed minutes must be zero or a positive number.")]
        [Display(Name = "Total Needed Minutes")]
        [Comment("Sum of the theoretical time required for all tasks performed that day.")]
        public decimal TotalNeededMinutes { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total worked minutes must be zero or a positive number.")]
        [Display(Name = "Total Worked Minutes")]
        [Comment("Total actual time worked by the employee on that day.")]
        public decimal TotalWorkedMinutes { get; set; }

        [Required]
        [Range(0, 200, ErrorMessage = "Efficiency must be between 0 and 200.")]
        [Display(Name = "Efficiency (%)")]
        [Comment("Calculated efficiency as (TotalNeededMinutes / TotalWorkedMinutes) * 100.")]
        public decimal EfficiencyPercentage { get; set; }
    }
}
