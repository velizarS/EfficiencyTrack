using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EfficiencyTrack.Data.Models
{
    [Comment("Represents a routing operation in the system.")]
    public class Routing : BaseEntity
    {
        [Required]
        [Display(Name = "Routing Code")]
        [Comment("The unique code for the routing operation.")]
        [StringLength(20, ErrorMessage = "Code cannot be longer than 20 characters.")]
        public string Code { get; set; } = null!;
        [Required]
        [Display(Name = "Description")]
        [Comment("The description of the routing operation.")]
        [StringLength(200, ErrorMessage = "Description cannot be longer than 200 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Zone")]
        [Comment("The zone where the routing operation is performed.")]
        [StringLength(100, ErrorMessage = "Zone name cannot be longer than 100 characters.")]
        public string Zone { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Minutes Per Piece")]
        [Comment("The time in minutes required to process one piece in this routing operation.")]
        [Range(0.01, 180, ErrorMessage = "Minutes per piece must be between 0.01 and 180 minutes.")]
        [Column(TypeName = "decimal(10,4)")]
        public decimal MinutesPerPiece { get; set; }

        [Required]
        [Display(Name = "Department")]
        [Comment("The department responsible for the routing operation.")]
        public Guid DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public Department Department { get; set; } = null!;

        public ICollection<Entry> Entries { get; set; } = [];
    }
}
