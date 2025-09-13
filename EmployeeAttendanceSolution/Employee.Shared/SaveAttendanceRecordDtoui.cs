using Domain.Enums;

namespace EmployeesModels.Shared;

public class SaveAttendanceRecordDtoui
{
    public string Code { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeSpan CheckIn { get; set; } = TimeSpan.Zero;
    public TimeSpan CheckOut { get; set; } = TimeSpan.Zero;
    public AttendanceStatus ActualStatus { get; set; }
    public AttendanceStatus PlannedStatus { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; }
    public string? Note { get; set; } = string.Empty;
}
public class SavePendingAttendanceCommand
{
    public List<SaveAttendanceRecordDtoui> PendingAttendance { get; set; } = new();
}