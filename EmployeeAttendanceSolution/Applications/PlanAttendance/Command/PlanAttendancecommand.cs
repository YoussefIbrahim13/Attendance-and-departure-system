using Domain.Enums;
using MediatR;

namespace Applications.PlanAttendance.Command;

public class PlanAttendancecommand: IRequest<(bool Success, string Message)>
{
    public string Code { get; set; }
    public List<DateTime> Dates { get; set; }
    public AttendanceStatus PlannedStatus { get; set; }

}