using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public abstract class EntryBaseViewModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "Кодът не може да е по-дълъг от 20 символа.")]
        [Display(Name = "Код на служител")]
        public string EmployeeCode { get; set; } = null!;

        [Required(ErrorMessage = "Код на операцията е задължителен.")]
        [Display(Name = "Код на операцията")]
        public string RoutingCode { get; set; } = null!;

        [Required(ErrorMessage = "Смяната е задължителна.")]
        [Display(Name = "Смяна")]
        public Guid ShiftId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Произведените бройки трябва да са положително число.")]
        [Display(Name = "Произведени бройки")]
        public int Pieces { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Бракът трябва да е положително число.")]
        [Display(Name = "Брак")]
        public int Scrap { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Отработените минути трябва да са положително число.")]
        [Display(Name = "Отработени минути")]
        public decimal WorkedMinutes { get; set; }

        [ScaffoldColumn(false)]
        public Guid EmployeeId { get; set; }

        [ScaffoldColumn(false)]
        public Guid RoutingId { get; set; }
    }
}
