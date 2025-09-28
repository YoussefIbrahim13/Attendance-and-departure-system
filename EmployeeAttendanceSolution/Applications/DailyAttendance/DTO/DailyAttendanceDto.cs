using Domain.Enums;

namespace Applications.DailyAttendance.DTO;

public class DailyAttendanceDto
{
    public string Code { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan CheckIn { get; set; } = TimeSpan.Zero;
    public TimeSpan CheckOut { get; set; } = TimeSpan.Zero;
    public AttendanceStatus ActualStatus { get; set; } = AttendanceStatus.No_status;
    public AttendanceStatus PlannedStatus { get; set; } = AttendanceStatus.No_status;
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public string Note { get; set; } = string.Empty;
}