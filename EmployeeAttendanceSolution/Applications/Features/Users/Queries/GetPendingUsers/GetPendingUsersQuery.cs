using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Users.Queries.GetPendingUsers
{
   
    public record GetPendingUsersQuery() : IRequest<IEnumerable<UserResponseDto>>;

}
