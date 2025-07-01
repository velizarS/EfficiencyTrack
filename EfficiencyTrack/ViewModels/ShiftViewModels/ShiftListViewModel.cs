namespace EfficiencyTrack.ViewModels.Shift
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class ShiftListViewModel
    {
        public List<ShiftViewModel> Shifts { get; set; } = new();
    }

}
