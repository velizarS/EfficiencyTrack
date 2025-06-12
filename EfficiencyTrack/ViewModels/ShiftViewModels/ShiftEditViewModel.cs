namespace EfficiencyTrack.ViewModels.Shift
{
    using System.ComponentModel.DataAnnotations;

    public class ShiftEditViewModel
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Смяна")]
        [RegularExpression(@"^[\p{L}\d\s\-']+$", ErrorMessage = "Името може да съдържа само букви, цифри, интервали, тирета и апострофи.")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Продължителност(в минути)")]
        [Range(1, 720, ErrorMessage = "Продължителността трябва да е между 1 и 720 минути.")]
        public int DurationMinutes { get; set; }
    }
}
