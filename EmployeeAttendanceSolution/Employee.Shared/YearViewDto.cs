namespace EmployeesModels.Shared
{
    public class YearViewDto
        {
            public int Year { get; set; }
            public List<MonthSummaryDto> Months { get; set; } = new();
        }

}




