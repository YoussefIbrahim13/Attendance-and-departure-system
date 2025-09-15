using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.DeleteUser
{
  
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, OperationResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<OperationResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return new OperationResult { Success = false, Message = $"User with ID {request.UserId} not found" };

            await _userManager.DeleteAsync(user);

            return new OperationResult { Success = true, Message = "User deleted successfully" };
        }
    }

}
