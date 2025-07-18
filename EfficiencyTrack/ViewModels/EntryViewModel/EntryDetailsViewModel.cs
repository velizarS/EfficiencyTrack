using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public class EntryDetailsViewModel : EntryDisplayBaseViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Дата на запис")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Служител")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Смяна")]
        public Guid ShiftId { get; set; }

        [Display(Name = "Операция (Routing)")]
        public Guid RoutingId { get; set; }

        [Display(Name = "Необходими минути")]
        public decimal RequiredMinutes { get; set; }
    }

}
