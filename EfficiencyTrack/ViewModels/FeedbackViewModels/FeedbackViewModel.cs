using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.FeedbackViewModels
{
    public class FeedbackViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Име на служител")]
        public string? EmployeeName { get; set; }

        [Display(Name = "Дата на създаване")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Съобщение")]
        public string Message { get; set; } = null!;

        public bool IsHandled { get; set; }
    }

}
