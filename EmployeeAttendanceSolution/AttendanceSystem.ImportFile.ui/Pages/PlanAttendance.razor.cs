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
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Microsoft.AspNetCore.Components.Forms;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Domain.Enums;


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
            private HashSet<DayOfWeek> selectedDays = new();
            private bool showRangeOptions = false;

        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject] NavigationManager Navigation { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // 1- تحقق من الرول
            var authState = await AuthenticationStateTask;
            var user = authState.User;

            string[] roles = { "Admin" };

            if (!user.Identity.IsAuthenticated || !roles.Any(role => user.IsInRole(role)))
            {
                Navigation.NavigateTo("/access-denied");
                return;
            }

            // 2- لو اليوزر مسموحله، هات الموظفين
            employees = await AttendanceService.GetAllEmployeesAsync();
        }
        private Task<IEnumerable<EmployeeDto>> SearchEmployee(string value, CancellationToken token)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return Task.FromResult(employees.AsEnumerable());
                value = value.Trim().ToLower();
                var result = employees.Where(e =>
                    (!string.IsNullOrEmpty(e.Name) && e.Name.ToLower().Contains(value)) ||
                    (!string.IsNullOrEmpty(e.Code) && e.Code.ToLower().Contains(value))
                );
                return Task.FromResult(result);
            }

            private async Task SavePlan()
            {
                if (selectedEmployee == null || string.IsNullOrEmpty(selectedEmployee.Code) || selectedStatus == null)
                {
                    Snackbar.Add("Please select employee and status.", Severity.Error);
                    return;
                }
                var days = new List<DateTime>();
                if (singleDate != null)
                {
                    days.Add(singleDate.Value.Date);
                }
                else if (showRangeOptions && dateRange.Start != null && dateRange.End != null)
                {
                    if (selectedDays != null && selectedDays.Count > 0)
                    {
                        for (var d = dateRange.Start.Value.Date; d <= dateRange.End.Value.Date; d = d.AddDays(1))
                        {
                            if (selectedDays.Contains(d.DayOfWeek))
                                days.Add(d);
                        }
                    }
                    else
                    {
                        for (var d = dateRange.Start.Value.Date; d <= dateRange.End.Value.Date; d = d.AddDays(1))
                            days.Add(d);
                    }
                }
                if (days.Count == 0)
                {
                    Snackbar.Add("Please select a day, range, or days of week.", Severity.Error);
                    return;
                }
                isLoading = true;
                StateHasChanged();
                var ok = await AttendanceService.PlanAttendanceAsync(selectedEmployee.Code, days, selectedStatus.Value);
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

            // تحديث الأيام المختارة عند تغيير CheckBox لأي يوم
            private void OnDayOfWeekChanged(DayOfWeek day, bool isChecked)
            {
                if (isChecked)
                    selectedDays.Add(day);
                else
                    selectedDays.Remove(day);
            }
        }
}