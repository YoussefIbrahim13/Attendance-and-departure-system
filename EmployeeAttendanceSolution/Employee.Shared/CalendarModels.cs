namespace EmployeesModels.Shared
{
    public record AttendanceDayStatus(
       string EmployeeId,
       DateTime Date,
       AttendanceStatus Status,
       string? Note
    );

}




