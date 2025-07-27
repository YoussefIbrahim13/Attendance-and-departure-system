namespace EmployeesModels.Shared
{
    public class MonthViewDto
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public List<CalendarDayDto> Days { get; set; } = new();
        }

}




