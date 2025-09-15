using EmployeesModels.Shared;
using EmployeesModels.Shared.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, OperationResult<List<UserResponseDto>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<OperationResult<List<UserResponseDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Load all users with their employees
                var users = await _db.Users
                    .Include(u => u.Employee)
                    .ToListAsync(cancellationToken);

                // 2️⃣ Load all roles and user-role assignments in one query
                var roles = await _db.Roles.ToListAsync(cancellationToken);
                var userRoles = await _db.UserRoles.ToListAsync(cancellationToken);

                // 3️⃣ Map users to DTOs
                var userDtos = users.Select(u =>
                {
                    var rolesForUser = userRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToList();

                    return new UserResponseDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        Name = u.Name,
                        Department = u.Employee?.Department.ToString(),
                        Position = u.Employee?.Position.ToString(),
                        IsApproved = u.IsApproved,
                        IsLockedByAdmin = u.IsLockedByAdmin,
                        Roles = rolesForUser
                    };
                }).ToList();

                return new OperationResult<List<UserResponseDto>>
                {
                    Success = true,
                    Data = userDtos
                };
            }
            catch (Exception ex)
            {
                return new OperationResult<List<UserResponseDto>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
