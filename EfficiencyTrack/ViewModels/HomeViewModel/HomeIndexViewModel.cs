using EfficiencyTrack.Data.Models;

namespace EfficiencyTrack.ViewModels.HomeViewModel
{
    public class HomeIndexViewModel
    {
        public IEnumerable<DailyEfficiency> Top10Today { get; set; } = new List<DailyEfficiency>();
        public IEnumerable<DailyEfficiency> Top10ThisMonth { get; set; } = new List<DailyEfficiency>();
    }
}
