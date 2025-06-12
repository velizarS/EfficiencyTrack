using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.Routing
{
    public class RoutingDetailViewModel : BaseDetailViewModel
    {
        [Display(Name = "Код")]
        public string Code { get; set; } = null!;
        [Display(Name = "Описание")]
        public string Description { get; set; } = null!;

        [Display(Name = "Зона")]
        public string Zone { get; set; } = null!;

        [Display(Name = "Време за една бройка в минутри")]
        public decimal MinutesPerPiece { get; set; }

        [Display(Name = "Отдел")]
        public Guid DepartmentId { get; set; }

        [Display(Name = "Отдел")]
        public string DepartmentName { get; set; } = null!;
    }

}
