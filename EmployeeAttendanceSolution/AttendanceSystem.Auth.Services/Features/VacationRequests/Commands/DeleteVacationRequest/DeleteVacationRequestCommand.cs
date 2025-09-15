using EmployeesModels.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.DeleteVacationRequest
{
    public record DeleteVacationRequestCommand(string RequestId) : IRequest<OperationResult>;
}
