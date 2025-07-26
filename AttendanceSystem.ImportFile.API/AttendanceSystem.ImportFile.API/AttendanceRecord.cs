using AttendanceSystem.ImportFile.API.Shared;

namespace AttendanceSystem.ImportFile.API
{
    public class AttendanceRecord
    {
        // تم حذف Id، وسيتم استخدام EmployeeId + Date كمفتاح أساسي مركب
        public string EmployeeId { get; set; }
        public DateTime Date { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public AttendanceStatus Status { get; set; } 
        public string? Note { get; set; } // ملاحظة HR
    }
}
