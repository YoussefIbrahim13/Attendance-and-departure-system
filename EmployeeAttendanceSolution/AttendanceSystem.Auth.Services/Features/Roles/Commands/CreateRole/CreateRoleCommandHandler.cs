using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AttendanceSystem.Auth.Services.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleResult>
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public CreateRoleCommandHandler(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<RoleResult> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
                return new RoleResult { Errors = new[] { new IdentityError { Description = "Role name cannot be empty" } } };

            if (await _roleManager.RoleExistsAsync(request.RoleName))
                return new RoleResult { Errors = new[] { new IdentityError { Description = $"Role '{request.RoleName}' already exists" } } };

            if (!Enum.TryParse<EmployeesModels.Shared.Roles>(request.RoleName, true, out var roleType))
                return new RoleResult { Errors = new[] { new IdentityError { Description = $"Invalid role. Valid roles are: {string.Join(", ", Enum.GetNames(typeof(EmployeesModels.Shared.Roles)))}" } } };

            var role = new ApplicationRole
            {
                Name = request.RoleName,
                NormalizedName = request.RoleName.ToUpper(),
                RoleType = roleType
            };

            var result = await _roleManager.CreateAsync(role);

            return result.Succeeded
                ? new RoleResult { Id = role.Id, Name = role.Name, RoleType = role.RoleType }
                : new RoleResult { Errors = result.Errors };
        }
    }

}
