using Domain.Enums;

namespace EmployeesModels.Shared
{
    public record AttendanceDayStatus(
       string Code,
       DateTime Date,
       AttendanceStatus ActualStatus ,
       AttendanceStatus? PlannedStatus,
       ApprovalStatus? ApprovalStatus,
       string? Note
    );

}




