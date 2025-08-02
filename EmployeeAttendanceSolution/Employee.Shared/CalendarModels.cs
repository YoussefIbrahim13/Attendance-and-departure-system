namespace EmployeesModels.Shared
{
    public record AttendanceDayStatus(
       string EmployeeId,
       DateTime Date,
       AttendanceStatus ActualStatus ,
       AttendanceStatus? PlannedStatus,
       ApprovalStatus? ApprovalStatus,
       string? Note
    );

}




