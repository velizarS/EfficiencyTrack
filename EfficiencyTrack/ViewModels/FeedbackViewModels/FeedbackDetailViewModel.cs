using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.FeedbackViewModels
{
    public class FeedbackDetailViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Име на служител")]
        public string EmployeeName { get; set; } = null!;

        [Display(Name = "Съобщение")]
        public string Message { get; set; } = null!;

        [Display(Name = "Дата на създаване")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Обработено")]
        public bool IsHandled { get; set; }

        [Display(Name = "Дата на обработка")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime? HandledAt { get; set; }
    }
}
