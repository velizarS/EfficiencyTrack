namespace EfficiencyTrack.Services.DTOs
{
    using System;

    namespace EfficiencyTrack.Services.DTOs
    {
        public class DailyEfficiencyDto
        {
            public Guid Id { get; set; }

            public DateTime Date { get; set; }

            public string EmployeeCode { get; set; } = null!;

            public string EmployeeFullName { get; set; } = null!;

            public decimal TotalWorkedMinutes { get; set; }

            public decimal TotalNeddedMinutes { get; set; }

            public string ShiftName { get; set; } = null!;

            public decimal EfficiencyPercentage { get; set; }

            public List<EntryDto> Entries { get; set; } = new();
        }
    }

}
