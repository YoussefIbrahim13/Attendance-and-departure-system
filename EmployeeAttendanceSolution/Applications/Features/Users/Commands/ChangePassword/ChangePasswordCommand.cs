using Domain.Comman;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.ChangePassword
{
    
    public record ChangePasswordCommand(string UserId, string NewPassword) : IRequest<OperationResult>;

}
