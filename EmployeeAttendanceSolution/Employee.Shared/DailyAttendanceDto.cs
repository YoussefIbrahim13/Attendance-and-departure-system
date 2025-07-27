namespace EmployeesModels.Shared
{
    public class DailyAttendanceDto
        {
            public string EmployeeId { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public string CheckIn { get; set; } = string.Empty;
            public string CheckOut { get; set; } = string.Empty;
            public AttendanceStatus Status { get; set; }
            public string? Note { get; set; }
        }

}




