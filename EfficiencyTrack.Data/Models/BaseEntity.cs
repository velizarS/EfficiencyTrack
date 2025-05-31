using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.Data.Models
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Comment("When is created")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        [Comment("By who is created")]
        public string CreatedBy { get; set; } = null!;

        [Comment("When is modified")]
        public DateTime? ModifiedOn { get; set; }

        [Comment("By who is modified")]
        public string? ModifiedBy { get; set; }

        [Comment("Is deleted")]
        public bool IsDeleted { get; set; } = false;

        [Comment("By who is deleted")]
        public string DeletedBy { get; set; } =null!;
    }
    
}

