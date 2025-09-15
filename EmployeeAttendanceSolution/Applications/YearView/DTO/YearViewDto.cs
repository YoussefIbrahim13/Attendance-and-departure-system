namespace Applications.YearView.DTO;

public class YearViewDto
{
    public int Year { get; set; }
    public List<MonthSummaryDto> Months { get; set; } = new();
}