using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public enum HrStatusConfirmation
    {
        Pending, // HR has not yet confirmed the attendance status
        Confirmed, // HR has confirmed the attendance status
        Rejected, // HR has rejected the attendance status
        cancel // HR has canceled the attendance status
    }
}
