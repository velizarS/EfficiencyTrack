using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.RoutingViewModels
{
    public class RoutingDetailViewModel : RoutingBaseViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Отдел")]
        public string DepartmentName { get; set; } = null!;
    }
}
