namespace EfficiencyTrack.ViewModels.EmployeeViewModels
{
    public class EmployeeDetailViewModel : EmployeeDisplayBaseViewModel
    {
        public Guid Id { get; set; }

        public Guid? ShiftManagerUserId { get; set; }

        public Guid DepartmentId { get; set; }
    }
}
