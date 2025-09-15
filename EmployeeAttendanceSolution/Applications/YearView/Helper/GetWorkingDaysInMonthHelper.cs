namespace Applications.YearView.Helper;

public static class GetWorkingDaysInMonthHelper
{
    public static int GetWorkingDaysInMonth(int year, int month)
    {
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        int workingDays = 0;

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday)
            {
                workingDays++;
            }
        }

        return workingDays;
    }
}