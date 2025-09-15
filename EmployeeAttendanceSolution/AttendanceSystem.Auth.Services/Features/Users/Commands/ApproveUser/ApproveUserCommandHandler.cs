using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.ApproveUser
{
    public class ApproveUserCommandHandler : IRequestHandler<ApproveUserCommand, OperationResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ApproveUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<OperationResult> Handle(ApproveUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return new OperationResult { Success = false, Message = $"User with ID {request.UserId} not found" };

            user.IsApproved = true;
            await _userManager.UpdateAsync(user);

            return new OperationResult { Success = true, Message = "User approved successfully" };
        }
    }

}
