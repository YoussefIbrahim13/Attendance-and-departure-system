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
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using Blazored.LocalStorage;
using Domain.Entities;
using Domain.Enums;


namespace AttendanceSystem.ImportFile.ui.Pages
{
    public partial class DayView
    {
        [Parameter] public string DateString { get; set; } = string.Empty;

        private List<DailyAttendanceDto> dayData = new();
        private List<DailyAttendanceDto> filteredEmployees = new();
        private bool isLoading = true;
        private DateTime selectedDate;
        private string searchTerm = string.Empty;
        private string _selectedStatusString = string.Empty;
        private string selectedStatusString
        {
            get => _selectedStatusString;
            set
            {
                if (_selectedStatusString != value)
                {
                    _selectedStatusString = value;
                    FilterEmployees(); // Call filter whenever the value changes
                }
            }
        }

        // Editing controls
        private bool _readOnly = false;
        private bool _isCellEditMode = false;
        private bool _editTriggerRowClick = false;
        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        protected override async Task OnInitializedAsync()
        {
            // 1- Parse Date
            if (!DateTime.TryParse(DateString, out selectedDate))
                selectedDate = DateTime.Today;

            // 2- Authorization Check
            var authState = await AuthenticationStateTask;
            var user = authState.User;

            string[] roles = { "Admin" };

            if (!user.Identity.IsAuthenticated || !roles.Any(role => user.IsInRole(role)))
            {
                Navigation.NavigateTo("/access-denied");
                return;
            }

            // 3- Load data if Authorized
            await LoadDayData();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (DateTime.TryParse(DateString, out selectedDate))
                await LoadDayData();
        }

        private async Task LoadDayData()
        {
            isLoading = true;
            try
            {
                dayData = await AttendanceService.GetDayViewAsync(selectedDate);
                filteredEmployees = new List<DailyAttendanceDto>(dayData);
                // Snackbar.Add("Attendance data loaded successfully", Severity.Success);
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading day data: {ex.Message}", Severity.Error);
                Console.WriteLine($"❌ Error loading day data: {ex.Message}");
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private void FilterEmployees()
        {
            filteredEmployees = dayData.Where(emp =>
            {
                bool matchesSearch = string.IsNullOrEmpty(searchTerm) ||
                    emp.EmployeeName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    emp.Department.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    emp.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);

                bool matchesStatus = string.IsNullOrEmpty(selectedStatusString) ||
                    emp.ActualStatus.ToString().Equals(selectedStatusString, StringComparison.OrdinalIgnoreCase);

                return matchesSearch && matchesStatus;
            }).ToList();

            StateHasChanged(); // Ensure UI updates
        }
        // ✅ Navigation Helpers
        private void PreviousDay() => Navigation.NavigateTo($"/calendar/day/{selectedDate.AddDays(-1):yyyy-MM-dd}");
        private void NextDay() => Navigation.NavigateTo($"/calendar/day/{selectedDate.AddDays(1):yyyy-MM-dd}");
        private void GoToMonthView() => Navigation.NavigateTo($"/calendar/month/{selectedDate.Year}/{selectedDate.Month}");

        // ✅ UI Helpers
        private string GetDateTitle() => selectedDate.ToString("dddd, MMMM dd, yyyy");
        private string GetDayOfWeek() => selectedDate.ToString("dddd");

        private string GetStatusClass(AttendanceStatus status) => status switch
        {
            AttendanceStatus.No_status =>"no status",
            AttendanceStatus.Present => "present",
            AttendanceStatus.Absent => "absent",
            AttendanceStatus.Vacation => "vacation",
            AttendanceStatus.WorkFromHome => "work-from-home",
            _ => "absent"
        };

        private string GetStatusIcon(AttendanceStatus status) => status switch
        {
            // AttendanceStatus.Present => Icons.Material.Filled.CheckCircle,
            // AttendanceStatus.Absent => Icons.Material.Filled.Cancel,
            // AttendanceStatus.Vacation => Icons.Material.Filled.CalendarToday,
            // AttendanceStatus.WorkFromHome => Icons.Material.Filled.Home,
            // _ => Icons.Material.Filled.Help
            AttendanceStatus.No_status => "○",
            AttendanceStatus.Present => "✓",
            AttendanceStatus.Absent => "✗",
            AttendanceStatus.Vacation => "🏖️",
            AttendanceStatus.WorkFromHome => "🏠",
            _ => "?"
        };

        private string FormatTimeSpan(TimeSpan time)
        {
            // Returns format like "5:30" (no leading zero, no seconds)
            return $"{(time.Hours == 0 ? "0" : time.Hours.ToString())}:{time.Minutes:00}";
        }


        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "??";
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                : parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
        }

        // حذف الدالة الزائدة، والإبقاء فقط على النسخة الصحيحة:
        private string CalculateWorkingHours(TimeSpan checkIn, TimeSpan checkOut)
        {
            var workingTime = checkOut - checkIn;
            return workingTime.TotalMinutes > 0
                ? $"{workingTime.Hours}h {workingTime.Minutes}m"
                : "--";
        }

        private int GetStatusCount(AttendanceStatus status) => dayData.Count(emp => emp.ActualStatus == status);

        // ✅ Summary Card Helper
        RenderFragment SummaryCard(string label, AttendanceStatus status, string icon, string cssClass) => builder =>
          {
              builder.OpenComponent<MudItem>(0);
              builder.AddAttribute(1, "Xs", 12);
              builder.AddAttribute(2, "Sm", 6);
              builder.AddAttribute(3, "Md", 3);
              builder.AddAttribute(4, "ChildContent", (RenderFragment)(childBuilder =>
              {
                  childBuilder.OpenComponent<MudPaper>(5);
                  childBuilder.AddAttribute(6, "Class", $"summary-card {cssClass} p-4 d-flex flex-column align-center");
                  childBuilder.AddAttribute(7, "Elevation", 2);
                  childBuilder.AddAttribute(8, "ChildContent", (RenderFragment)(contentBuilder =>
                  {
                      // Icon
                      contentBuilder.OpenComponent<MudIcon>(9);
                      contentBuilder.AddAttribute(10, "Icon", icon);
                      contentBuilder.AddAttribute(11, "Size", Size.Large);
                      contentBuilder.AddAttribute(12, "Class", "summary-icon mb-2");
                      contentBuilder.CloseComponent();

                      // Number
                      contentBuilder.OpenElement(13, "div");
                      contentBuilder.AddAttribute(14, "class", "summary-number mud-typography-h5");
                      contentBuilder.AddContent(15, GetStatusCount(status));
                      contentBuilder.CloseElement();

                      // Label
                      contentBuilder.OpenElement(16, "div");
                      contentBuilder.AddAttribute(17, "class", "summary-label mud-typography-body2");
                      contentBuilder.AddContent(18, label);
                      contentBuilder.CloseElement();
                  }));
                  childBuilder.CloseComponent();
              }));
              builder.CloseComponent();
          };

        // ✅ Editing Events
        private void StartedEditingItem(DailyAttendanceDto item)
        {
            Console.WriteLine($"Started editing: {item.EmployeeName}");
        }

        private void CanceledEditingItem(DailyAttendanceDto item)
        {
            Console.WriteLine($"Canceled editing: {item.EmployeeName}");
        }

        private async void CommittedItemChanges(DailyAttendanceDto item)
        {
            // if (item.CheckOut < item.CheckIn)
            // {
            //     Snackbar.Add("Check-out time must be after check-in time", Severity.Warning);
            //     return;
            // }
            try
            {
                AttendanceRecord employeeAttendanceRequest = new AttendanceRecord
                {
                    Code = item.Code,
                    Date = selectedDate,
                    ActualStatus = item.ActualStatus,
                    PlannedStatus = item.PlannedStatus,
                    ApprovalStatus = item.ApprovalStatus,
                    CheckIn = item.CheckIn,
                    CheckOut = item.CheckOut,
                    Note = item.Note
                };
                var result = await AttendanceService.UpdateEmployeeAttendanceRecordAsync(employeeAttendanceRequest);
                if (result)
                {
                    Snackbar.Add($"Successfully updated attendance", Severity.Success);
                    await LoadDayData(); // Refresh the data
                }
                else
                {
                    Snackbar.Add($"Failed to update attendance for {item.EmployeeName}", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error updating attendance: {ex.Message}", Severity.Error);
                Console.WriteLine($"Error updating attendance: {ex.Message}");
            }
        }
    }
}