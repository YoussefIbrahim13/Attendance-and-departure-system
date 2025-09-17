using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure_.DBContext;

namespace AttendanceSystem.Auth.Services.Features.Users.Queries.GetPendingUsers
{
    public class GetPendingUsersQueryHandler : IRequestHandler<GetPendingUsersQuery, IEnumerable<UserResponseDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public GetPendingUsersQueryHandler(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IEnumerable<UserResponseDto>> Handle(GetPendingUsersQuery request, CancellationToken cancellationToken)
        {
            // Load pending users and their employees
            var users = await _db.Users
                .Include(u => u.Employee)
                .Where(u => !u.IsApproved)
                .ToListAsync(cancellationToken);

            // Load roles and user-role assignments
            var roles = await _db.Roles.ToListAsync(cancellationToken);
            var userRoles = await _db.UserRoles.ToListAsync(cancellationToken);

            var result = users.Select(u =>
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
            });

            return result;
        }
    }
}
