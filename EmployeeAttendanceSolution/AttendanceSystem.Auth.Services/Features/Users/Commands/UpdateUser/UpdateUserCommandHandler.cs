using Domain.Entities;
using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Infrastructure_.DBContext;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db; // <-- Inject DbContext to access Employee

        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<UserResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            // 1. Find user
            var user = await _userManager.FindByIdAsync(request.Id);
            if (user == null)
                return new UserResult { Errors = new[] { new IdentityError { Description = "User not found" } } };

            // 2. Email validation and update
            if (!string.IsNullOrEmpty(dto.Email) &&
                !string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
            {
                var emailValidator = new EmailAddressAttribute();
                if (!emailValidator.IsValid(dto.Email))
                    return new UserResult { Errors = new[] { new IdentityError { Description = "Invalid email format" } } };

                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                    return new UserResult { Errors = new[] { new IdentityError { Description = "Email is already in use" } } };

                user.Email = dto.Email;
                user.UserName = dto.Email;
                user.NormalizedEmail = _userManager.NormalizeEmail(dto.Email);
                user.NormalizedUserName = _userManager.NormalizeEmail(dto.Email);
            }

            // 3. Update allowed properties
            user.Name = dto.Name ?? user.Name;
            user.IsApproved = dto.IsApproved;

            // ❌ DO NOT set Department/Position on ApplicationUser anymore
            // ✅ Instead, update Employee entity if needed
            if (user.EmployeeId != null && user.EmployeeId != Guid.Empty)
            {
                var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == user.EmployeeId, cancellationToken);
                if (employee != null)
                {
                    if (!string.IsNullOrEmpty(dto.Department) &&
                        Enum.TryParse<DepartmentEnum>(dto.Department, out var departmentEnum))
                        employee.Department = departmentEnum;

                    if (!string.IsNullOrEmpty(dto.Position) &&
                        Enum.TryParse<PositionEnum>(dto.Position, out var positionEnum))
                        employee.Position = positionEnum;
                }
            }

            // 4. Save changes
            var updateResult = await _userManager.UpdateAsync(user);
            await _db.SaveChangesAsync(cancellationToken);

            if (!updateResult.Succeeded)
                return new UserResult { Errors = updateResult.Errors };

            // 5. Reload Employee for DTO
            var updatedEmployee = user.EmployeeId != Guid.Empty
                ? await _db.Employees.FirstOrDefaultAsync(e => e.Id == user.EmployeeId, cancellationToken)
                : null;

            // 6. Return updated user info
            return new UserResult
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Department = updatedEmployee?.Department.ToString(),
                Position = updatedEmployee?.Position.ToString(),
                IsApproved = user.IsApproved,
                IsLockedByAdmin = user.IsLockedByAdmin,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }
    }
}
