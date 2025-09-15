using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.UnlockUser
{
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, OperationResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UnlockUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<OperationResult> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return new OperationResult { Success = false, Message = "User not found" };

            user.IsLockedByAdmin = false;
            user.AccessFailedCount = 0;
            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);

            return new OperationResult { Success = true, Message = "User unlocked successfully" };
        }
    }

}
