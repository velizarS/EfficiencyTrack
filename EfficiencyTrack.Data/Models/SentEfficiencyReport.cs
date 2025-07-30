using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.Data.Models
{
    public class SentEfficiencyReport
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime DateSent { get; set; } 
    }
}
