using EmployeesModels.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.ApproveVacationRequest
{
    public record ApproveVacationRequestCommand(string RequestId, string ApprovedBy) : IRequest<OperationResult>;
}
