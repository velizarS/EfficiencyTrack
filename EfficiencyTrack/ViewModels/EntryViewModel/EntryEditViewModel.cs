using System;
using System.ComponentModel.DataAnnotations;


namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public class EntryEditViewModel : EntryBaseViewModel
    {
        public Guid Id { get; set; }

        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Кодът трябва да съдържа само главни букви и цифри.")]
        public new string EmployeeCode { get; set; } = null!;

        [RegularExpression(@"^[A-Z0-9]+$", ErrorMessage = "Кодът трябва да съдържа само главни букви и цифри.")]
        public new string RoutingCode { get; set; } = null!;
    }

}