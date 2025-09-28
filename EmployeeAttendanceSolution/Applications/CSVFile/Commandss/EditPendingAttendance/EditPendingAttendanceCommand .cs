using Applications.CSVFile.DTOS.AttendanceRecord;
using Domain.Entities;
using System.Windows.Input;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Applications.CSVFile.Commandss.EditPendingAttendance;

public class EditPendingAttendanceCommand: IRequest<string>
{
    public List<AttendanceRecord> PendingAttendance { get; set; } = new();
    public EditAttendanceRecordDto Dto { get; set; } = new();
    public string Code { get; set; } = string.Empty;
    public DateTime Date { get; set; }


}