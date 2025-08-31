using EmployeesModels.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.DeleteUser
{
   
    public record DeleteUserCommand(string UserId) : IRequest<OperationResult>;

}
