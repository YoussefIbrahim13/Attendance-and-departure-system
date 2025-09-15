using Applications.UpdateAttendanceRecord.DTOS;
using Domain.Enums;
using MediatR;

namespace Applications.UpdateAttendanceRecord.Commands;

public class UpdateAttendanceRecordcommand : IRequest<(bool Success, string Message)>
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