namespace EfficiencyTrack.ViewModels.HomeViewModel
{
    public class HomeIndexViewModel
    {
        public List<TopEfficiencyViewModel> Top10Today { get; set; } = [];
        public List<TopEfficiencyViewModel> Top10ThisMonth { get; set; } = [];
    }
}
