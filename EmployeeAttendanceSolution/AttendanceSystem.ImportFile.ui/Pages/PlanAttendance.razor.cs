//namespace AttendanceSystem.ImportFile.ui.Shared
//{
//    public enum AttendanceStatus
//    {
//        Present,
//        Absent,
//        Vacation,
//        WorkFromHome,
//        //Sick,
//        //Late,
//        //EarlyLeave
//    }


using AttendanceSystem.ImportFile.ui.Services;
using EmployeesModels.Shared;
using MudBlazor;

namespace AttendanceSystem.ImportFile.ui.Pages
{
    public partial class PlanAttendance
    {
        private List<EmployeeDto> employees = new();
        private EmployeeDto? selectedEmployee;
        private AttendanceStatus? selectedStatus;
        private DateTime? singleDate;
        private DateRange dateRange = new DateRange(null, null);
        private string? message;
        private bool? isSuccess = null;
        private bool isLoading = false;

        protected override async Task OnInitializedAsync()
        {
            employees = await AttendanceService.GetAllEmployeesAsync();
        }

        private Task<IEnumerable<EmployeeDto>> SearchEmployee(string value, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Task.FromResult(employees.AsEnumerable());
            value = value.Trim().ToLower();
            var result = employees.Where(e =>
                (!string.IsNullOrEmpty(e.Name) && e.Name.ToLower().Contains(value)) ||
                (!string.IsNullOrEmpty(e.Id) && e.Id.ToLower().Contains(value))
            );
            return Task.FromResult(result);
        }




        private async Task SavePlan()
        {
            PlanAttendanceService.SavePlan(selectedEmployee, selectedStatus, Snackbar);
            if (selectedEmployee == null || string.IsNullOrEmpty(selectedEmployee.Id) || selectedStatus == null)
            {
                Snackbar.Add("Please select employee and status.", Severity.Error);
                return;
            }
            var days = new List<DateTime>();
            if (singleDate != null)
            {
                days.Add(singleDate.Value.Date);
            }
            else if (dateRange.Start != null && dateRange.End != null)
            {
                for (var d = dateRange.Start.Value.Date; d <= dateRange.End.Value.Date; d = d.AddDays(1))
                    days.Add(d);
            }
            if (days.Count == 0)
            {
                Snackbar.Add("Please select a day or range.", Severity.Error);
                return;
            }
            isLoading = true;
            StateHasChanged();
            var ok = await AttendanceService.PlanAttendanceAsync(selectedEmployee.Id, days, selectedStatus.Value);
            isLoading = false;
            if (ok)
            {
                Snackbar.Add("Attendance plan saved successfully.", Severity.Success);
            }
            else
            {
                Snackbar.Add("Failed to save plan.", Severity.Error);
            }
        }
        private void ClearRange()
        {
            dateRange = new DateRange(null, null);
        }
    }
}