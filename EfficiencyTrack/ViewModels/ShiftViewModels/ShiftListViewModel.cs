namespace EfficiencyTrack.ViewModels.Shift
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ShiftListViewModel
    {
        public List<ShiftViewModel> Shifts { get; set; } = new();

        [Display(Name = "Търсене")]
        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }
        public bool SortAsc { get; set; } = true;
    }

}
