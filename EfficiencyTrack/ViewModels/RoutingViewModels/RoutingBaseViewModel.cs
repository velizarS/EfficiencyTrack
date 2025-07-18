using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.RoutingViewModels
{
    public abstract class RoutingBaseViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "Кодът не може да е по-дълъг от 20 символа.")]
        [Display(Name = "Код")]
        public string Code { get; set; } = null!;

        [Required]
        [StringLength(200, ErrorMessage = "Описанието не може да е по-дълго от 200 символа.")]
        [Display(Name = "Описание")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Зона")]
        public string Zone { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 180, ErrorMessage = "Времето трябва да е между 0.01 и 180 минути.")]
        [Display(Name = "Време за една бройка в минути")]
        public decimal MinutesPerPiece { get; set; }

        [Required]
        [Display(Name = "Отдел")]
        public Guid DepartmentId { get; set; }
    }
}
