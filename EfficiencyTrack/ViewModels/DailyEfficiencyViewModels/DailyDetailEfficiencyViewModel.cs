using EfficiencyTrack.ViewModels.EntryViewModel;
using System.ComponentModel;

namespace EfficiencyTrack.ViewModels.DailyEfficiencyViewModels
{
    public class DailyDetailEfficiencyViewModel : DailyEfficiencyBaseViewModel
    {
        public Guid Id { get; set; }

        [DisplayName("Общо необходими минути")]
        public decimal TotalNeededMinutes { get; set; }

        [DisplayName("Детайли на операциите")]
        public List<EntryDetailsViewModel> DetailEntries { get; set; } = new List<EntryDetailsViewModel>();
    }
}
