using AttendanceSystem.Auth.Services.Features.Users.Commands.SendRandomPassword;
using EmployeesModels.Shared;
using EmployeesModels.Shared.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // 🔹 Needed for FirstOrDefaultAsync
using System.ComponentModel.DataAnnotations;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.AddUser
{
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, UserResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly IMediator _mediator; // ✅ use mediator to call SendRandomPasswordCommand

        public AddUserCommandHandler(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext db,
            IMediator mediator)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            _mediator = mediator;
        }

        public async Task<UserResult> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // ✅ Validate email
            if (!new EmailAddressAttribute().IsValid(dto.Email))
            {
                return new UserResult
                {
                    Errors = new[] { new IdentityError { Description = "Invalid email format" } }
                };
            }

            // ✅ Find the Employee by code
            var employee = await _db.Employees
                .FirstOrDefaultAsync(e => e.Code == dto.EmployeeCode, cancellationToken);

            if (employee == null)
            {
                return new UserResult
                {
                    Errors = new[] { new IdentityError { Description = $"Employee with code {dto.EmployeeCode} not found" } }
                };
            }

            // ✅ Prevent duplicate user
            var existingUser = await _db.Users
                .FirstOrDefaultAsync(u => u.EmployeeId == employee.Id, cancellationToken);

            if (existingUser != null)
            {
                return new UserResult
                {
                    Errors = new[] { new IdentityError { Description = $"Employee {employee.Name} already has a user account." } }
                };
            }

            // ✅ Ask SendRandomPasswordCommand to generate password & send email
            var passwordResult = await _mediator.Send(new SendRandomPasswordCommand(dto.Email), cancellationToken);

            if (string.IsNullOrEmpty(passwordResult.Password))
            {
                return new UserResult
                {
                    Errors = new[] { new IdentityError { Description = "Failed to generate password" } }
                };
            }

            var randomPassword = passwordResult.Password;

            // ✅ Create new ApplicationUser
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = employee.Name,   // from Employee
                IsApproved = false,
                EmployeeId = employee.Id
            };

            var result = await _userManager.CreateAsync(user, randomPassword);
            if (!result.Succeeded)
                return new UserResult { Errors = result.Errors };

            // ✅ Ensure role exists
            if (!await _roleManager.RoleExistsAsync(request.RoleName))
            {
                return new UserResult
                {
                    Errors = new[] { new IdentityError { Description = "Role does not exist" } }
                };
            }

            await _userManager.AddToRoleAsync(user, request.RoleName);

            // ✅ Return Department & Position from Employee
            return new UserResult
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = employee.Name,
                Department = employee.Department.ToString(),
                Position = employee.Position.ToString(),
                IsApproved = user.IsApproved,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }
    }
}
