using EmployeesModels.Shared;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Users.Queries.GetAllUsers
{
   
    public record GetAllUsersQuery() : IRequest<OperationResult<List<UserResponseDto>>>;

}
