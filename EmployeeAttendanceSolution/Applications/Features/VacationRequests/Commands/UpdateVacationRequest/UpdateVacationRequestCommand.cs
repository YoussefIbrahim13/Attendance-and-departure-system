using Domain.Comman;
using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.UpdateVacationRequest
{
    public record UpdateVacationRequestCommand(string RequestId, UpdateVacationRequestDto Dto)
    : IRequest<OperationResult>;
}
