using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.Routing
{
    public class RoutingViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Код")]
        public string Code { get; set; } = null!;

        [Display(Name = "Време за една бройка")]
        public decimal MinutesPerPiece { get; set; }

        [Display(Name = "Отдел")]
        public string DepartmentName { get; set; } = null!;

        [Display(Name = "Зона")]
        public string Zone { get; set; } = null!;
    }
}
