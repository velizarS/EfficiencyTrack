using EfficiencyTrack.ViewModels.EntryViewModel;
using System.ComponentModel;

namespace EfficiencyTrack.ViewModels.DailyEfficiencyViewModels
{
    public class DailyDetailEfficiencyViewModel
    {
        public Guid Id { get; set; }

        [DisplayName("Дата")]
        public DateTime Date { get; set; }

        [DisplayName("Код на служител")]
        public string EmployeeCode { get; set; } = null!;

        [DisplayName("Служител")]
        public string EmployeeFullName { get; set; } = null!;

        [DisplayName("Общо отработени минути")]
        public decimal TotalWorkedMinutes { get; set; }

        [DisplayName("Смяна")]
        public string ShiftName { get; set; } = null!;

        [DisplayName("Ефективност (%)")]
        public decimal EfficiencyPercentage { get; set; }

        [DisplayName("Детайли на операциите")]
        public List<EntryDetailsViewModel> DetailEntries { get; set; } = new List<EntryDetailsViewModel>();
    }
}
