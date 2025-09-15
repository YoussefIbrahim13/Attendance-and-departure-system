using Applications.Employees.Commands.UploadProfileImagecommand;
using Applications.Employees.DTO.EmployeeDtos;
using AutoMapper;
using Domain.Entities;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.UpdateAttendanceRecord.Commands;

public class UpdateAttendanceRecordcommandHandler :  IRequestHandler<UpdateAttendanceRecordcommand, (bool Success, string Message)>
{
    private readonly AppDbcontext _db;
    private readonly IMapper _mapper;

    public UpdateAttendanceRecordcommandHandler(AppDbcontext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;

    }
    public async Task<(bool Success, string Message)> Handle(UpdateAttendanceRecordcommand command, CancellationToken cancellationToken)
    {
        // ابحث عن السجل الموجود حسب Code و Date
        var existing = await _db.AttendanceRecords
            .FirstOrDefaultAsync(a => a.Code == command.Code && a.Date == command.Date, cancellationToken);

        if (existing == null)
        {
            // لو مفيش سجل موجود، أنشئ جديد
            var newRecord = new AttendanceRecord
            {
                Code = command.Code,
                Date = command.Date,
                CheckIn = command.CheckIn,
                CheckOut = command.CheckOut,
                ActualStatus = command.ActualStatus,
                PlannedStatus = command.PlannedStatus,
                ApprovalStatus = command.ApprovalStatus,
                Note = command.Note
            };

            _db.AttendanceRecords.Add(newRecord);
        }
        else
        {
            // لو موجود، حدث الحقول مباشرة
            existing.CheckIn = command.CheckIn;
            existing.CheckOut = command.CheckOut;
            existing.ActualStatus = command.ActualStatus;
            existing.PlannedStatus = command.PlannedStatus;
            existing.ApprovalStatus = command.ApprovalStatus;
            existing.Note = command.Note;
        }

        await _db.SaveChangesAsync(cancellationToken);

        return (true, "Attendance record updated successfully");
    }

}
