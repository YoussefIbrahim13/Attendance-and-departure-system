using Applications.CSVFile.DTOS.AttendanceRecord;
using AutoMapper;
using Domain.Entities;
using Infrastructure_;
using Infrastructure_.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.CSVFile.Commandss.SavePendingAttendanceCommand;

public class SavePendingAttendanceCommandHandler : IRequestHandler<SavePendingAttendanceCommand, string>
{
    private readonly IMapper _mapper;
    private readonly ApplicationDbContext _db;

    public SavePendingAttendanceCommandHandler(IMapper mapper, ApplicationDbContext db)
    {
        _mapper = mapper;
        _db = db;
    }

    public async Task<string> Handle(SavePendingAttendanceCommand command, CancellationToken cancellationToken)
    {
        var pendingAttendance = command.PendingAttendance;

        if (pendingAttendance == null || pendingAttendance.Count == 0)
            return "No data";

        var employeeIds = pendingAttendance.Select(x => x.Code).ToList();
        var dates = pendingAttendance.Select(x => x.Date).ToList();

        var existingRecords = await _db.AttendanceRecords
            .Where(x => employeeIds.Contains(x.Code) && dates.Contains(x.Date))
            .ToListAsync(cancellationToken);

        foreach (var recDto in pendingAttendance)
        {
            var existing = existingRecords
                .FirstOrDefault(x => x.Code == recDto.Code && x.Date == recDto.Date);

            if (existing != null)
            {
                _mapper.Map(recDto, existing);
            }
            else
            {
                var newRecord = _mapper.Map<AttendanceRecord>(recDto);
                _db.AttendanceRecords.Add(newRecord);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return "Saved successfully";
    }

}