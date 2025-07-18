using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.ShiftViewModels
{
    public class ShiftEditViewModel : ShiftBaseViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }

}
