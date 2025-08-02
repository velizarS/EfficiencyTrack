namespace EfficiencyTrack.ViewModels.FeedbackViewModels
{
    public class FeedbackListViewModel
    {
        public List<FeedbackViewModel> Feedbacks { get; set; } = new List<FeedbackViewModel>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
