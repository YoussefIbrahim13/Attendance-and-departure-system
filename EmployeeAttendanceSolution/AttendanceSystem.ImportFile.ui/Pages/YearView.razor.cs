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
{
    public partial class YearView
    {
        [Parameter] public int Year { get; set; }

        private YearViewDto? yearData;
        private bool isLoading = true;
        private int currentYear;

        protected override async Task OnInitializedAsync()
        {
            currentYear = Year > 0 ? Year : DateTime.Today.Year;
            await LoadYearData();
        }

        protected override async Task OnParametersSetAsync()
        {
            if (Year > 0) currentYear = Year;
            await LoadYearData();
        }

        private async Task LoadYearData()
        {
            isLoading = true;
            try
            {
                yearData = await AttendanceService.GetYearViewAsync(currentYear);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading year data: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        private void PreviousYear() => Navigation.NavigateTo($"/calendar/year/{--currentYear}");
        private void NextYear() => Navigation.NavigateTo($"/calendar/year/{++currentYear}");
        private void GoToCurrentMonth() => Navigation.NavigateTo($"/calendar/month/{DateTime.Today.Year}/{DateTime.Today.Month}");
        private void GoToMonth(int month) => Navigation.NavigateTo($"/calendar/month/{currentYear}/{month}");
        private bool IsCurrentMonth(int month) => currentYear == DateTime.Today.Year && month == DateTime.Today.Month;

        private int GetTotalWorkingDays() => yearData?.Months.Sum(m => m.TotalWorkingDays) ?? 0;
        private double GetAverageAttendance() => yearData?.Months.Any() == true ? Math.Round(yearData.Months.Average(m => m.AverageAttendance), 1) : 0;
        private string GetBestMonth() => yearData?.Months.Any() == true ? yearData.Months.OrderByDescending(m => m.AverageAttendance).First().MonthName : "N/A";
        private string GetWorstMonth() => yearData?.Months.Any() == true ? yearData.Months.OrderBy(m => m.AverageAttendance).First().MonthName : "N/A";

        private string GetMonthCardClass(int month)
        {
            var baseClass = "month-card";
            if (IsCurrentMonth(month))
            {
                return $"{baseClass} current-month";
            }
            return baseClass;
        }

        private Color GetAttendanceColor(double attendance)
        {
            // if (attendance >= 80) return Color.Success;
            // if (attendance >= 60) return Color.Warning;
            return Color.Primary;
        }
    }
}