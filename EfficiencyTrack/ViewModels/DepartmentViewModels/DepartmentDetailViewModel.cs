namespace EfficiencyTrack.ViewModels.DepartmentViewModels
{
    public class DepartmentDetailViewModel : DepartmentBaseViewModel
    {
        public Guid Id { get; set; }

        public List<EmployeeSimpleViewModel> Employees { get; set; } = [];
    }
}
