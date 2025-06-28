namespace EfficiencyTrack.ViewModels.Shift
{
    using System.ComponentModel.DataAnnotations;

    public class ShiftCreateViewModel
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "Смяна")]
        [RegularExpression(@"^[a-zA-Z\d\s\-']+$", ErrorMessage = "Името може да съдържа само букви, цифри, интервали, тирета и апострофи.")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Продължителност(в минути)")]
        [Range(1, 720, ErrorMessage = "Продължителността трябва да е между 1 и 720 минути.")]
        public int DurationMinutes { get; set; }
    }
}
