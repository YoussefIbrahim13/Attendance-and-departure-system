using AttendanceSystem.ImportFile.ui.Services;
using Domain.Enums;
using EmployeesModels.Shared;
using MudBlazor;

namespace AttendanceSystem.ImportFile.ui.Pages.nvvm
{
    public class PlanAttendanceService : IPlanAttendanceService
    {
        public async Task SavePlan(EmployeeDto? selectedEmployee, AttendanceStatus? selectedStatus, ISnackbar Snackbar)
        {
            if (selectedEmployee == null || string.IsNullOrEmpty(selectedEmployee.Code) || selectedStatus == null)
            {
                Snackbar.Add("Please select employee and status.", Severity.Error);
                return;
            }
            //var days = new List<DateTime>();
            //if (singleDate != null)
            //{
            //    days.Add(singleDate.Value.Date);
            //}
            //else if (dateRange.Start != null && dateRange.End != null)
            //{
            //    for (var d = dateRange.Start.Value.Date; d <= dateRange.End.Value.Date; d = d.AddDays(1))
            //        days.Add(d);
            //}
            //if (days.Count == 0)
            //{
            //    Snackbar.Add("Please select a day or range.", Severity.Error);
            //    return;
            //}
            //isLoading = true;
            //StateHasChanged();
            //var ok = await AttendanceService.PlanAttendanceAsync(selectedEmployee.Id, days, selectedStatus.Value);
            //isLoading = false;
            //if (ok)
            //{
            //    Snackbar.Add("Attendance plan saved successfully.", Severity.Success);
            //}
            //else
            //{
            //    Snackbar.Add("Failed to save plan.", Severity.Error);
            //}
        }
    }
}
