namespace EfficiencyTrack.ViewModels.DailyEfficiencyViewModels
{
    public class DailyEfficiencyListViewModel
    {
        public List<DailyEfficiencyViewModel> DailyEfficiencies { get; set; } = new List<DailyEfficiencyViewModel>();

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
