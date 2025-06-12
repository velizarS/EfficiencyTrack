using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.FeedbackViewModels
{
    public class FeedbackCreateViewModel
    {
        [Required(ErrorMessage = "Името на служителя е задължително.")]
        [StringLength(100, ErrorMessage = "Името не може да е по-дълго от 100 символа.")]
        [Display(Name = "Име на служител")]
        public string? EmployeeName { get; set; } 

        [Required(ErrorMessage = "Съобщението е задължително.")]
        [StringLength(4000, ErrorMessage = "Съобщението не може да е по-дълго от 4000 символа.")]
        [Display(Name = "Съобщение")]
        public string Message { get; set; } = null!;
    }

}
