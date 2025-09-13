using Applications.CSVFile.Commandss.EditPendingAttendance;
using AutoMapper;
using MediatR;

namespace Applications.CSVFile.Commandss.EditPendingAttendance;

public class EditPendingAttendanceCommandHandler : IRequestHandler<EditPendingAttendanceCommand, string>
{
    private readonly IMapper _mapper;

    public EditPendingAttendanceCommandHandler(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task<string> Handle(EditPendingAttendanceCommand command, CancellationToken cancellationToken)
    {
        var record = command.PendingAttendance
            .FirstOrDefault(x => x.Code == command.Code && x.Date.Date == command.Date.Date);
        if (record == null)
            return "Attendance record not found in pending data.";

        _mapper.Map(command.Dto, record);

        await Task.CompletedTask;

        return "Pending attendance record updated successfully.";
    }
}
