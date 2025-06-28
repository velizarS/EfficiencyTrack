using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public class EntryCreateViewModel
    {
        [Required]
        [Display(Name = "Код на служител")]
        public string EmployeeCode { get; set; } 

        [Required]
        [Display(Name = "Код на операция (Routing)")]
        public string RoutingCode { get; set; } 

        [Required]
        [Display(Name = "Смяна")]
        public Guid ShiftId { get; set; } 

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Брой произведени трябва да е по-голямо от 1.")]
        [Display(Name = "Произведени бройки")]
        public int Pieces { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Брой брак трябва да е нула или положително число.")]
        [Display(Name = "Брой брак")]
        public int Scrap { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Отработени минути трябва да е положително число.")]
        [Display(Name = "Отработени минути")]
        public decimal WorkedMinutes { get; set; }
    }
}
