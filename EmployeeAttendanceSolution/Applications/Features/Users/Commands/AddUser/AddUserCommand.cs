using Domain.Comman;
using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.AddUser
{
    public class AddUserCommand : IRequest<UserResult>
    {
        public UserCreateDto Dto { get; set; } = new();
        public string RoleName { get; set; } = string.Empty;

        public AddUserCommand(UserCreateDto dto, string roleName)
        {
            Dto = dto;
            RoleName = roleName;
        }
    }
}
