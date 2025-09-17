using Domain.Entities;
using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Roles.Queries.GetRoleById
{
    public class GetRoleByIdQueryHandler : IRequestHandler<GetRoleByIdQuery, RoleResult>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public GetRoleByIdQueryHandler(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<RoleResult> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await _roleManager.FindByIdAsync(request.Id);
            return role == null
                ? new RoleResult { Errors = new[] { new IdentityError { Description = "Role not found" } } }
                : new RoleResult { Id = role.Id, Name = role.Name, RoleType = role.RoleType };
        }
    }

}
