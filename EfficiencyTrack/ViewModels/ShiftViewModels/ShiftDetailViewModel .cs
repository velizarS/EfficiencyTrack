using EfficiencyTrack.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.Shift
{
    public class ShiftDetailViewModel : BaseDetailViewModel
    {
        [Display(Name = "Смяна")]
        public string Name { get; set; } = null!;

        [Display(Name = "Продължителност (в минути)")]
        public int DurationMinutes { get; set; }
    }
}
