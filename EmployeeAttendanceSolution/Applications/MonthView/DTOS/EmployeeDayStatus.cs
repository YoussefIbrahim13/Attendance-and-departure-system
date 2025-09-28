using Domain.Enums;

namespace Applications.MonthView.DTOS;

public class EmployeeDayStatus
{
    public string Code { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;
    public AttendanceStatus ActualStatus { get; set; } = AttendanceStatus.No_status;
    public string? Note { get; set; }
}