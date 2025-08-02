namespace EmployeesModels.Shared
{
    public class EditAttendanceDto
        {
            public string EmployeeId { get; set; }
            public DateTime Date { get; set; }
            public TimeSpan CheckIn { get; set; }
            public TimeSpan CheckOut { get; set; }
           public AttendanceStatus ActualStatus { get; set; }
            public AttendanceStatus PlannedStatus { get; set; }
            public ApprovalStatus ApprovalStatus { get; set; }
            public string? Note { get; set; }
        }

}




