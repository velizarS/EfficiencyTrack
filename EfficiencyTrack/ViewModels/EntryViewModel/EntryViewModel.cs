using System.ComponentModel.DataAnnotations;

namespace EfficiencyTrack.ViewModels.EntryViewModel
{
    public class EntryViewModel
    {
        public Guid Id { get; set; }

        [Display(Name = "Дата на запис")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Display(Name = "Служител")]
        public Guid EmployeeId { get; set; }

        [Display(Name = "Код на служител")]
        public string? EmployeeCode{ get; set; }

        [Display(Name = "Име на служител")]
        public string? EmployeeName { get; set; }

        [Display(Name = "Операция (Routing)")]
        public Guid RoutingId { get; set; }

        [Display(Name = "Име на операция")]
        public string? RoutingName { get; set; } 

        [Display(Name = "Ефективност (%)")]
        public decimal EfficiencyForOperation { get; set; }
    }
}
