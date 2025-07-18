namespace EfficiencyTrack.Services.DTOs
{
    public class TopEfficiencyDto
    {
        public string FullName { get; set; } = null!;
        public decimal EfficiencyPercentage { get; set; }
        public string DepartmentName { get; set; } = null!;
        public string ShiftManagerName { get; set; } = null!;
        public string? ShiftName { get; set; }
    }
}
