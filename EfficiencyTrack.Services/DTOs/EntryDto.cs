using EfficiencyTrack.Data.Models;

namespace EfficiencyTrack.Services.DTOs
{
    public class EntryDto
    {
        public DateTime Date { get; set; }

        public Guid EmployeeId { get; set; }

        public Guid RoutingId { get; set; }

        public string? RoutingName { get; set; }

        public int Pieces { get; set; }

        public decimal WorkedMinutes { get; set; }

        public decimal EfficiencyForOperation { get; set; }
    }
}
