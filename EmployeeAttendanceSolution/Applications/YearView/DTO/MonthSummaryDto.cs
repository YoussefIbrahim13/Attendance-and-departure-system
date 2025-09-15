namespace Applications.YearView.DTO;

public class MonthSummaryDto
{
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int TotalWorkingDays { get; set; }
    public double AverageAttendance { get; set; }
}