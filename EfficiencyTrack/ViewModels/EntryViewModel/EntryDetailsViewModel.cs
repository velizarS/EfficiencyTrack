using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public class EntryDetailsViewModel : BaseDetailViewModel
    {
        [Display(Name = "Дата на запис")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Служител")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Смяна")]
        public Guid ShiftId { get; set; }

        [Display(Name = "Операция (Routing)")]
        public Guid RoutingId { get; set; }

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
