using Domain.Enums;

namespace EmployeesModels.Shared
{
    public class CalendarDayDto
    {
        public DateTime Date { get; set; }
        public List<EmployeeDayStatus> TopEmployees { get; set; } = new(); // First 3-4 employees for month view
        public List<EmployeeDayStatus> AllEmployees { get; set; } = new(); // All employees for the day
        public int TotalEmployees { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }

        public IEnumerable<EmployeeDayStatus> GetSelectedEmployee(string searchTerm, AttendanceStatus? selectedStatus, int size)
        {
            if(AllEmployees is null)
            {
                return new List<EmployeeDayStatus>();
            }
            return AllEmployees.Where(e => (!selectedStatus.HasValue || e.ActualStatus == selectedStatus)
            &&   (string.IsNullOrWhiteSpace(searchTerm) || $"{e.EmployeeName} -- {e.Code}" == searchTerm))
                                               .Take(size);

            // .Where(e => (!selectedStatus.HasValue || e.ActualStatus == selectedStatus)
                                          //      && (string.IsNullOrWhiteSpace(selectedEmployeeName) || $"{e.EmployeeName} -- {e.Code}" == selectedEmployeeName))
                                        //    .Take(3)
        }
    }

}




