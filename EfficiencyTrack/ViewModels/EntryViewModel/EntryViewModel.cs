using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public class EntryViewModel
    {
        [Display(Name = "Дата на запис")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Служител")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Операция (Routing)")]
        public Guid RoutingId { get; set; }

        [Display(Name = "Ефективност (%)")]
        public decimal EfficiencyForOperation { get; set; }

        
    }
}
