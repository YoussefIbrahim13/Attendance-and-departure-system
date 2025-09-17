using Applications.MonthView.Helper.DateHelper;
using Applications.YearView.DTO;
using Applications.YearView.Helper;

using Domain.Enums;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using MediatR;
using Infrastructure.DBContext;

namespace Applications.YearView.Querys;

public class GetYearViewqueryHandler : IRequestHandler<GetYearViewquery, YearViewDto>
{
    private readonly ApplicationDbContext _db;

    public GetYearViewqueryHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<YearViewDto> Handle(GetYearViewquery query, CancellationToken cancellationToken)
    {
        var yearViewDto = new YearViewDto
        {
            Year = query.Year,
            Months = new List<MonthSummaryDto>()
        };

        var totalEmployees = await _db.Employees.CountAsync(cancellationToken);

        for (int month = 1; month <= 12; month++)
        {
            var startDate = new DateTime(query.Year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var attendanceData = await _db.AttendanceRecords
                .Where(ar => ar.Date >= startDate && ar.Date <= endDate)
                .ToListAsync(cancellationToken);

            int workingDays = GetWorkingDaysInMonthHelper.GetWorkingDaysInMonth(query.Year, month);

            int totalPossibleAttendance = totalEmployees * workingDays;
            int actualAttendance = attendanceData.Count(ar => ar.ActualStatus == AttendanceStatus.Present);

            double averageAttendance = totalPossibleAttendance > 0
                ? (double)actualAttendance / totalPossibleAttendance * 100
                : 0;

            yearViewDto.Months.Add(new MonthSummaryDto
            {
                Month = month,
                MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                TotalWorkingDays = workingDays,
                AverageAttendance = Math.Round(averageAttendance, 2)
            });
        }

        return yearViewDto;
    }
}