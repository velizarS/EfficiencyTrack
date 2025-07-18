using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.Data.Models
{
    [Comment("Represents a work shift in the system.")]
    public class Shift : BaseEntity
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "Shift Name")]
        [Comment("The name of the shift.")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Shift Duration (minutes)")]
        [Comment("The duration of the shift in total minutes.")]
        [Range(1, 720, ErrorMessage = "Duration must be between 1 and 720 minutes.")]
        public int DurationMinutes { get; set; }

        public ICollection<Entry> Entries { get; set; } = [];

    }
}
