using AttendanceSystem.ImportFile.ui.Services;
using EmployeesModels.Shared;
using MudBlazor;

namespace AttendanceSystem.ImportFile.ui.Pages.nvvm
{
    public interface IPlanAttendanceService
    {
        Task SavePlan(EmployeeDto? selectedEmployee, AttendanceStatus? selectedStatus, ISnackbar Snackbar);
    }
}