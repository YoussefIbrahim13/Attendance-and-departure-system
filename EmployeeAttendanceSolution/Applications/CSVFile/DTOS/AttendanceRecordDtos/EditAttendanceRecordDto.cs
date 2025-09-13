using Domain.Enums;

namespace Applications.CSVFile.DTOS.AttendanceRecord;

public class EditAttendanceRecordDto
{

    public TimeSpan CheckIn { get; set; } = TimeSpan.Zero;
    public TimeSpan CheckOut { get; set; } = TimeSpan.Zero;
    public AttendanceStatus ActualStatus { get; set; }
    public AttendanceStatus PlannedStatus { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; }
    public string? Note { get; set; }= string.Empty;
}