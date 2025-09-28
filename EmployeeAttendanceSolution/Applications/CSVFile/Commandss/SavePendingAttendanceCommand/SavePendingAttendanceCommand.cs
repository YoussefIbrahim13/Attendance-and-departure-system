using Applications.CSVFile.DTOS.AttendanceRecord;
using MediatR;

namespace Applications.CSVFile.Commandss.SavePendingAttendanceCommand;

public class SavePendingAttendanceCommand : IRequest<string>
{
    public List<SaveAttendanceRecordDto> PendingAttendance { get; set; } = new();


}                                                   