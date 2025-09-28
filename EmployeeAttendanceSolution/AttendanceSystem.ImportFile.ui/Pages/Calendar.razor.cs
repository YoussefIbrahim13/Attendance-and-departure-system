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

namespace AttendanceSystem.ImportFile.ui.Pages
{
    public partial class Calendar
    {
        private string hoverClass = "";
        private void GoToMonthView() => Navigation.NavigateTo($"/calendar/month/{DateTime.Today.Year}/{DateTime.Today.Month}");
        private void GoToYearView() => Navigation.NavigateTo($"/calendar/year/{DateTime.Today.Year}");
        private void GoToTodayView() => Navigation.NavigateTo($"/calendar/day/{DateTime.Today:yyyy-MM-dd}");
        private void GoToCurrentMonth() => Navigation.NavigateTo($"/calendar/month/{DateTime.Today.Year}/{DateTime.Today.Month}");
        private void GoToCurrentYear() => Navigation.NavigateTo($"/calendar/year/{DateTime.Today.Year}");
        private void GoToToday() => Navigation.NavigateTo($"/calendar/day/{DateTime.Today:yyyy-MM-dd}");
        private void GoToImport() => Navigation.NavigateTo("/ImportAttendance");


    }
}