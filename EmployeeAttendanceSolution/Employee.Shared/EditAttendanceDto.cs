namespace EmployeesModels.Shared
{
    public class EditAttendanceDto
        {
            public string EmployeeId { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan CheckIn { get; set; } = TimeSpan.Zero;
            public TimeSpan CheckOut { get; set; } = TimeSpan.Zero;
           public AttendanceStatus ActualStatus { get; set; }
            public AttendanceStatus PlannedStatus { get; set; }
            public ApprovalStatus ApprovalStatus { get; set; }
            public string? Note { get; set; }
        }

}




