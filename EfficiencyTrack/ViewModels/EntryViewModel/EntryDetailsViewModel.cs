using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public class EntryDetailsViewModel : BaseDetailViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Дата на запис")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Служител")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Код на служител")]
        public string? EmployeeCode { get; set; }

        [Display(Name = "Име на служител")]
        public string? EmployeeName { get; set; }

        [Display(Name = "Смяна")]
        public Guid ShiftId { get; set; }

        [Display(Name = "Операция (Routing)")]
        public Guid RoutingId { get; set; }

        [Display(Name = "Код на операцията")]
        public string? RoutingName { get; set; }

        [Display(Name = "Произведени бройки")]
        public int Pieces { get; set; }

        [Display(Name = "Брой брак")]
        public int Scrap { get; set; }

        [Display(Name = "Отработени минути")]
        public decimal WorkedMinutes { get; set; }

        [Display(Name = "Необходими минути")]
        public decimal RequiredMinutes { get; set; }

        [Display(Name = "Ефективност (%)")]
        public decimal EfficiencyForOperation { get; set; }
    }
}
