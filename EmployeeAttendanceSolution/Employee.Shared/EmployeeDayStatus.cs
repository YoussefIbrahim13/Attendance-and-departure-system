namespace EmployeesModels.Shared
{
    public class EmployeeDayStatus
        {
            public string EmployeeId { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;
           public AttendanceStatus ActualStatus { get; set; }
         public AttendanceStatus PlannedStatus { get; set; }
          public ApprovalStatus ApprovalStatus { get; set; }
    public string? Note { get; set; }
        }

}




