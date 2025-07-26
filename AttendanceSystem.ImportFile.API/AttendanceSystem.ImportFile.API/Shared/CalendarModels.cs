namespace AttendanceSystem.ImportFile.API.Shared
{
        public enum AttendanceStatus
        {
            Present,
            Absent,
            Vacation,
            WorkFromHome,
            //Sick,
            //Late,
            //EarlyLeave
        }


        public class DailyAttendanceDto
        {
            public string EmployeeId { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;
            public string Department { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public string CheckIn { get; set; } = string.Empty;
            public string CheckOut { get; set; } = string.Empty;
            public AttendanceStatus Status { get; set; }
            public string? Note { get; set; }
        }

        public class MonthlyAttendanceDto
        {
            public DateTime Date { get; set; }
            public List<EmployeeDayStatus> Employees { get; set; } = new();
            public int TotalEmployees { get; set; }
            public int PresentCount { get; set; }
            public int AbsentCount { get; set; }
        }

        public class EmployeeDayStatus
        {
            public string EmployeeId { get; set; } = string.Empty;
            public string EmployeeName { get; set; } = string.Empty;
            public AttendanceStatus Status { get; set; }
            public string? Note { get; set; }
        }

        public class CalendarDayDto
        {
            public DateTime Date { get; set; }
            public List<EmployeeDayStatus> TopEmployees { get; set; } = new(); // First 3-4 employees for month view
            public int TotalEmployees { get; set; }
            public int PresentCount { get; set; }
            public int AbsentCount { get; set; }
        }

        public class MonthViewDto
        {
            public int Year { get; set; }
            public int Month { get; set; }
            public List<CalendarDayDto> Days { get; set; } = new();
        }

        public class YearViewDto
        {
            public int Year { get; set; }
            public List<MonthSummaryDto> Months { get; set; } = new();
        }

        public class MonthSummaryDto
        {
            public int Month { get; set; }
            public string MonthName { get; set; } = string.Empty;
            public int TotalWorkingDays { get; set; }
            public double AverageAttendance { get; set; }
        }

        ////////////////////////////////////
        public class Employee
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Department { get; set; }
            public string Position { get; set; }
        }
        public record AttendanceDayStatus(
           string EmployeeId,
           DateTime Date,
           AttendanceStatus Status,
           string? Note
        );

        //public class AttendanceRecord
        //{
        //    public string EmployeeId { get; set; }
        //    public DateTime Date { get; set; }
        //    public string CheckIn { get; set; }
        //    public string CheckOut { get; set; }
        //    public AttendanceStatus Status { get; set; } // Using the enum here
        //    public string? Note { get; set; }
        //}

        public class EditAttendanceDto
        {
            public string EmployeeId { get; set; }
            public DateTime Date { get; set; }
            public string CheckIn { get; set; }
            public string CheckOut { get; set; }
            public AttendanceStatus Status { get; set; } // Using the enum here
            public string? Note { get; set; }
        }

    }




