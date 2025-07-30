namespace EmployeesModels.Shared
{
    public class AttendanceRecord
        {
        public string EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan CheckIn { get; set; }
        public TimeSpan CheckOut { get; set; }
        public AttendanceStatus Status { get; set; } // Using the enum here
        public string? Note { get; set; }
        }

}




