namespace EfficiencyTrack.ViewModels.RoutingViewModels
{
    public class RoutingListViewModel
    {
        public List<RoutingViewModel> Routings { get; set; } = new List<RoutingViewModel>();

        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

}
