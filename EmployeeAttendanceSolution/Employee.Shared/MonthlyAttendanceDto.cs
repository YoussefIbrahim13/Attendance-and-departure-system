namespace EmployeesModels.Shared
{
    public class MonthlyAttendanceDto
        {
            public DateTime Date { get; set; }
            public List<EmployeeDayStatus> Employees { get; set; } = new();
            public int TotalEmployees { get; set; }
            public int PresentCount { get; set; }
            public int AbsentCount { get; set; }
        }

}




