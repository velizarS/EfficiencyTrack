using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public abstract class EntryDisplayBaseViewModel
    {
        [Display(Name = "Код на служител")]
        public string EmployeeCode { get; set; } = string.Empty;

        [Display(Name = "Име на служител")]
        public string EmployeeName { get; set; } = string.Empty;

        [Display(Name = "Име на операция")]
        public string RoutingName { get; set; } = string.Empty;

        [Display(Name = "Произведени бройки")]
        public int Pieces { get; set; }

        [Display(Name = "Брой брак")]
        public int Scrap { get; set; }

        [Display(Name = "Отработени минути")]
        public decimal WorkedMinutes { get; set; }

        [Display(Name = "Ефективност (%)")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal EfficiencyForOperation { get; set; }
    }

}
