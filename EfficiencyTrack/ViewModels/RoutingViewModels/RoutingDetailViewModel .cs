using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.RoutingViewModels
{
    public class RoutingEditViewModel : RoutingBaseViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Отдели")]
        public List<SelectListItem> AvailableDepartments { get; set; } = [];

        [Display(Name = "Зони")]
        public List<SelectListItem> AvailableZones { get; set; } = [];
    }
}
