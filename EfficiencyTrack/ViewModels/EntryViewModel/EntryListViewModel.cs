namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public class EntryListViewModel
    {
        public List<EntryViewModel> Entries { get; set; } = new List<EntryViewModel>();
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    }
}
