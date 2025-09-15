using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, OperationResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<OperationResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return new OperationResult { Success = false, Message = $"User with ID {request.UserId} not found" };

            if (await _userManager.CheckPasswordAsync(user, request.NewPassword))
                return new OperationResult { Success = false, Message = "New password cannot be the same as current password" };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            return result.Succeeded
                ? new OperationResult { Success = true, Message = "Password changed successfully" }
                : new OperationResult { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };
        }
    }

}
