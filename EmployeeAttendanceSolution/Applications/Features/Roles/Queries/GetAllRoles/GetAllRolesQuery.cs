using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Roles.Queries.GetAllRoles
{
 
    public record GetAllRolesQuery() : IRequest<IEnumerable<string>>;

}
