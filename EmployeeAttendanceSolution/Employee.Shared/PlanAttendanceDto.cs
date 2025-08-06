namespace EmployeesModels.Shared
{
    public class PlanAttendanceDto
    {
        public string Code { get; set; }
        public List<DateTime> Dates { get; set; } = new();
       
        public AttendanceStatus PlannedStatus { get; set; }
       
    }
}
