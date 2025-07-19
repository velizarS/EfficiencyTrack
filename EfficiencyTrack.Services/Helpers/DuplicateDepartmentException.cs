namespace EfficiencyTrack.Services.Helpers
{
    public class DuplicateDepartmentException : Exception
    {
        public DuplicateDepartmentException(string message) : base(message) { }
    }
}
