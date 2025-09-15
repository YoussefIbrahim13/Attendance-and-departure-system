namespace Applications.MonthView.DTOS;

public class CalendarDayDto
{
    public DateTime Date { get; set; }
    public List<EmployeeDayStatus> TopEmployees { get; set; } = new();
    public List<EmployeeDayStatus> AllEmployees { get; set; } = new();
    public int TotalEmployees { get; set; }
    public int PresentCount { get; set; }
    public int AbsentCount { get; set; }
}