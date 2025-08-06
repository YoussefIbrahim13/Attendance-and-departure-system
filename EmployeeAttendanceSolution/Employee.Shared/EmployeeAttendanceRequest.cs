using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public class EmployeeAttendanceRequest
    {
        public string Code { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public AttendanceStatus ActualStatus { get; set; }
         public AttendanceStatus PlannedStatus { get; set; }
        public ApprovalStatus ApprovalStatus { get; set; }
        public string? Note { get; set; }
    }
}
