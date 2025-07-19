using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfficiencyTrack.Data.Models
{
    [Comment("Represents a production entry for an employee, including details about the operation performed, pieces produced, and efficiency metrics.")]
    public class Entry : BaseEntity
    {
        [Display(Name = "Entry Date")]
        [Comment("The date when the entry was recorded.")]
        public DateTime Date { get; set; }

        [Required]
        [ForeignKey(nameof(Employee))]
        [Display(Name = "Employee")]
        [Comment("The employee associated with this entry.")]
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Routing))]
        [Display(Name = "Routing")]
        [Comment("The routing operation associated with this entry.")]
        public Guid RoutingId { get; set; }
        public Routing Routing { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Shift))]
        [Display(Name = "Shift")]
        [Comment("The shift during which this entry was recorded.")]
        public Guid ShiftId { get; set; }
        public Shift Shift { get; set; } = null!;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Pieces must be zero or a positive number.")]
        [Display(Name = "Pieces Produced")]
        [Comment("The number of pieces produced.")]
        public int Pieces { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Scrap must be zero or a positive number.")]
        [Display(Name = "Scrap Pieces")]
        [Comment("The number of scrap pieces produced.")]
        public int Scrap { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Worked minutes must be a positive number.")]
        [Display(Name = "Worked Minutes")]
        [Comment("The number of minutes worked during the entry.")]
        [Column(TypeName = "decimal(10,4)")]
        public decimal WorkedMinutes { get; set; }

        [Display(Name = "Required Minutes")]
        [Comment("Theoretical required time for this operation.")]
        [Column(TypeName = "decimal(10,4)")]
        public decimal RequiredMinutes { get; set; }

        [Range(0.01, 200, ErrorMessage = "Efficiency must be between 0.01 and 200.")]
        [Display(Name = "Efficiency (%)")]
        [Comment("The efficiency percentage for the operation.")]
        [Column(TypeName = "decimal(10,4)")]
        public decimal EfficiencyForOperation { get; set; }

    }

}

