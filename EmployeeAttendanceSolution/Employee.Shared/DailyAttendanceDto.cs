namespace EmployeesModels.Shared
{
    public class DailyAttendanceDto
        {
            public string EmployeeId { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public TimeSpan CheckIn { get; set; } = TimeSpan.Zero;
        public TimeSpan CheckOut { get; set; } = TimeSpan.Zero;
        public AttendanceStatus Status { get; set; }
            public string? Note { get; set; }
        }

}




