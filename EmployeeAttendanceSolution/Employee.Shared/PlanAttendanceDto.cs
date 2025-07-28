namespace EmployeesModels.Shared
{
    public class PlanAttendanceDto
    {
        public string EmployeeId { get; set; }
        public List<DateTime> Dates { get; set; } = new();
        public AttendanceStatus Status { get; set; }
    }
}
