using Domain.Entities;
using Domain.Enums;
using Infrastructure_;
using Infrastructure_.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.PlanAttendance.Command;

public class PlanAttendancecommandHandler : IRequestHandler<PlanAttendancecommand, (bool Success, string Message)>
{
    private readonly ApplicationDbContext _db;

    public PlanAttendancecommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<(bool Success, string Message)> Handle(PlanAttendancecommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Code) || command.Dates == null || command.Dates.Count == 0)
            return (false, "Invalid attendance plan data.");

        foreach (var date in command.Dates)
        {
            var record = await _db.AttendanceRecords
                .FirstOrDefaultAsync(x => x.Code == command.Code && x.Date == date, cancellationToken);

            if (record != null)
            {
                if (record.ActualStatus == record.PlannedStatus)
                    record.ActualStatus = command.PlannedStatus;

                record.PlannedStatus = command.PlannedStatus;
            }
            else
            {
                _db.AttendanceRecords.Add(new AttendanceRecord
                {
                    Code = command.Code,
                    Date = date,
                    PlannedStatus = command.PlannedStatus,
                    ActualStatus = command.PlannedStatus,
                    ApprovalStatus = ApprovalStatus.Pending,
                    CheckIn = TimeSpan.Zero,
                    CheckOut = TimeSpan.Zero
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return (true, "Attendance plan saved successfully.");
    }
}