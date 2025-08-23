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
using MudBlazor;

namespace AttendanceSystem.ImportFile.ui.Pages
// ...existing code...
{
        public partial class MonthView
        {
            // Autocomplete search for employee
            private string? selectedEmployeeName = null;
            private List<(string Name, string Id)> employeeNames = new();

            private Task<IEnumerable<string>> SearchEmployees(string value, CancellationToken token)
            {
                IEnumerable<string> result;
                if (string.IsNullOrWhiteSpace(value))
                    result = employeeNames.Select(e => $"{e.Name} -- {e.Id}");
                else
                    result = employeeNames
                        .Where(x => x.Name.Contains(value, StringComparison.OrdinalIgnoreCase) || x.Id.Contains(value, StringComparison.OrdinalIgnoreCase))
                        .Select(e => $"{e.Name} -- {e.Id}");
                return Task.FromResult(result);
            }
        [Parameter] public int Year { get; set; }
        [Parameter] public int Month { get; set; }
        [Parameter] public DateTime SelectedDate { get; set; }
        [CascadingParameter]
        public IDialogReference DialogReference { get; set; } = default!;

        private DateTime? dateFrom => SelectedDate;
        private DateTime? dateTo => SelectedDate;

    private MonthViewDto? monthData;
    private bool isLoading = true;
    private int currentYear;
    private int currentMonth;

    // Filter for employee status
    private AttendanceStatus? selectedStatus = null;

        protected override async Task OnInitializedAsync()
        {
            var today = DateTime.Today;
            currentYear = Year > 0 ? Year : today.Year;
            currentMonth = Month > 0 ? Month : today.Month;

            await LoadMonthData();
            // Fill employeeNames from monthData after loading
            if (monthData != null)
            {
                employeeNames = monthData.Days
                    .SelectMany(d => d.AllEmployees.Select(e => (e.EmployeeName, e.Code)))
                    .Distinct()
                    .OrderBy(x => x.EmployeeName)
                    .ToList();
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Year > 0) currentYear = Year;
            if (Month > 0) currentMonth = Month;
            await LoadMonthData();
            // Update employeeNames after loading monthData
            if (monthData != null)
            {
                employeeNames = monthData.Days
                    .SelectMany(d => d.AllEmployees.Select(e => (e.EmployeeName, e.Code)))
                    .Distinct()
                    .OrderBy(x => x.EmployeeName)
                    .ToList();
            }
        }

        private async Task LoadMonthData()
        {
            isLoading = true;
            // StateHasChanged(); // Force UI update to show loading state

            try
            {
                monthData = await AttendanceService.GetMonthViewAsync(currentYear, currentMonth);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading data: {ex.Message}", Severity.Error);
            }
            finally
            {
                isLoading = false;
                // StateHasChanged(); // Force UI update to hide loading state
            }
        }

        private string GetMonthYearTitle()
        {
            return new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");
        }

        private void PreviousMonth()
        {
            if (currentMonth == 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            else
            {
                currentMonth--;
            }
            Navigation.NavigateTo($"/calendar/month/{currentYear}/{currentMonth}");
        }

        private void NextMonth()
        {
            if (currentMonth == 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            else
            {
                currentMonth++;
            }
            Navigation.NavigateTo($"/calendar/month/{currentYear}/{currentMonth}");
        }

        private void GoToYearView()
        {
            Navigation.NavigateTo($"/calendar/year/{currentYear}");
        }

        private void GoToDayView(DateTime date)
        {
            Navigation.NavigateTo($"/calendar/day/{date:yyyy-MM-dd}");
        }

        private string GetCalendarDayClass(bool isCurrentMonth, bool isToday)
        {
            var classes = new List<string> { "calendar-day" };

            if (isCurrentMonth)
                classes.Add("current-month");
            else
                classes.Add("other-month");

            if (isToday)
                classes.Add("today");

            return string.Join(" ", classes);
        }

        private string GetEmployeeStatusClass(AttendanceStatus status)
        {
            return $"employee-item {GetStatusClass(status)}";
        }

        private string GetStatusClass(AttendanceStatus status)
        {
            return status switch
            {
                AttendanceStatus.No_status => "no status",
                AttendanceStatus.Present => "present",
                AttendanceStatus.Absent => "absent",
                AttendanceStatus.Vacation => "vacation",
                AttendanceStatus.WorkFromHome => "work-from-home",
                // AttendanceStatus.Late => "late",
                _ => "absent"
            };
        }

        private string GetStatusIndicatorClass(AttendanceStatus status)
        {
            return GetStatusClass(status);
        }

        private async Task OpenAddEmployeeDialog(DateTime selectedDate)
        {
            try
            {
                var parameters = new DialogParameters
                {
                    [nameof(AddEmployeeDialog.InitialDate)] = selectedDate,
                    [nameof(AddEmployeeDialog.DefaultDateFrom)] = selectedDate,
                    [nameof(AddEmployeeDialog.DefaultDateTo)] = selectedDate
                };
                var options = new DialogOptions
                {
                    CloseButton = true,
                    CloseOnEscapeKey = true,
                    MaxWidth = MaxWidth.Small,
                    FullWidth = true
                };

                var dialog = await DialogService.ShowAsync<AddEmployeeDialog>("Update Attendance Status", parameters, options);
                var result = await dialog.Result;

                if (!result.Canceled && result.Data is bool success && success)
                {
                    await LoadMonthData();
                }
                else
                {
                    await LoadMonthData();
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error: {ex.Message}", Severity.Error);
            }
        }

    }
}