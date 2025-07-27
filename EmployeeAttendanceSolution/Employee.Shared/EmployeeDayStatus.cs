namespace EmployeesModels.Shared
{
    public class EmployeeDayStatus
        {
            public string EmployeeId { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;
            public AttendanceStatus Status { get; set; }
            public string? Note { get; set; }
        }

}




