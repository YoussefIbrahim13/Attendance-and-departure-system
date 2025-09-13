namespace Applications.MonthView.DTOS;

public class MonthViewDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public List<CalendarDayDto> Days { get; set; } = new();
}