namespace EfficiencyTrack.ViewModels.HomeViewModel
{
    public class TopEfficiencyViewModel
    {
        public string FullName { get; set; } = string.Empty;
        public decimal EfficiencyPercentage { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? ShiftName { get; set; }
        public string? ShiftManagerName { get; set; }
    }
}
