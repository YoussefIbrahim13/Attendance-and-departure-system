using Domain.Enums;

namespace Applications.UpdateAttendanceRecord.DTOS;

public class UpdateAttendanceRecordDto
{
    public string Code { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan CheckIn { get; set; } = TimeSpan.Zero;
    public TimeSpan CheckOut { get; set; } = TimeSpan.Zero;
    public AttendanceStatus ActualStatus { get; set; }
    public AttendanceStatus PlannedStatus { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; }
    public string? Note { get; set; }
}