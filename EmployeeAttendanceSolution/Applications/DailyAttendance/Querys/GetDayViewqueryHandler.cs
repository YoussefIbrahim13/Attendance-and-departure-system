using Applications.DailyAttendance.DTO;
using Domain.Enums;
using Infrastructure;
using Infrastructure.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.DailyAttendance.Querys;

public class GetDayViewqueryHandler : IRequestHandler<GetDayViewquery, List<DailyAttendanceDto>>
{
    private readonly ApplicationDbContext _db;

    public GetDayViewqueryHandler(ApplicationDbContext db)
    {
        _db = db;
    }
    public async Task<List<DailyAttendanceDto>> Handle(GetDayViewquery query, CancellationToken cancellationToken)
    {


        var day = query.Date.Date;

        var employees = await _db.Employees.ToListAsync(cancellationToken);
        var attendanceData = await _db.AttendanceRecords
            .Where(ar => ar.Date.Date == day)
            .ToListAsync(cancellationToken);

        var dailyAttendance = employees.Select(emp =>
        {
            var attendance = attendanceData.FirstOrDefault(ar => ar.Code == emp.Code);

            return new DailyAttendanceDto
            {
                Code = emp.Code,
                EmployeeName = emp.Name,
                Department = emp.Department.ToString(),
                Date = day,
                CheckIn = attendance?.CheckIn ?? TimeSpan.Zero,
                CheckOut = attendance?.CheckOut ?? TimeSpan.Zero,
                ActualStatus = attendance?.ActualStatus ?? AttendanceStatus.No_status,
                PlannedStatus = attendance?.PlannedStatus ?? AttendanceStatus.No_status,
                ApprovalStatus = attendance?.ApprovalStatus ?? ApprovalStatus.Pending,
                Note = attendance?.Note ?? string.Empty
            };
        }).ToList();

        return dailyAttendance;
    }
}