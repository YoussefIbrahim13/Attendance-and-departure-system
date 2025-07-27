namespace EmployeesModels.Shared
{
    public class AttendanceRecord
        {
        public string EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public AttendanceStatus Status { get; set; } // Using the enum here
        public string? Note { get; set; }
        }

}




