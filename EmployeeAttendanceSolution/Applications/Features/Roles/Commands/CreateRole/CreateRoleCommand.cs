using Domain.Comman;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Roles.Commands.CreateRole
{
    public record CreateRoleCommand(string RoleName) : IRequest<RoleResult>;

}
