namespace EmployeesModels.Shared
{
    public class CalendarDayDto
        {
            public DateTime Date { get; set; }
            public List<EmployeeDayStatus> TopEmployees { get; set; } = new(); // First 3-4 employees for month view
            public int TotalEmployees { get; set; }
            public int PresentCount { get; set; }
            public int AbsentCount { get; set; }
        }

}




