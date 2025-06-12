using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.DailyEfficiencyViewModels
{
    public class DailyEfficiencyViewModel
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
    }
}
