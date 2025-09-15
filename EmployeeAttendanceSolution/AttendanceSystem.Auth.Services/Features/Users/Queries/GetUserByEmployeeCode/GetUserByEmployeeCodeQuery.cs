using EmployeesModels.Shared;
using EmployeesModels.Shared.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace AttendanceSystem.Auth.Services.Features.Users.Queries.GetUserByEmployeeCode
{
    // ✅ Request
    public class GetUserByEmployeeCodeQuery : IRequest<UserResult>
    {
        public string EmployeeCode { get; set; } = string.Empty;
    }

    // ✅ Handler
    public class GetUserByEmployeeCodeQueryHandler : IRequestHandler<GetUserByEmployeeCodeQuery, UserResult>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public GetUserByEmployeeCodeQueryHandler(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<UserResult> Handle(GetUserByEmployeeCodeQuery request, CancellationToken cancellationToken)
        {
            // 🔹 Find employee by code
            var employee = await _db.Employees
                .FirstOrDefaultAsync(e => e.Code == request.EmployeeCode, cancellationToken);

            if (employee == null)
            {
                return new UserResult
                {
                    Errors = new[] { new IdentityError { Description = $"Employee with code {request.EmployeeCode} not found." } }
                };
            }

            // 🔹 Find user linked to that employee
            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.EmployeeId == employee.Id, cancellationToken);

            if (user == null)
            {
                return new UserResult
                {
                    Errors = new[] { new IdentityError { Description = $"No user account found for employee {employee.Name}." } }
                };
            }

            // 🔹 Get roles
            var roles = await _userManager.GetRolesAsync(user);

            // 🔹 Return UserResult
            return new UserResult
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = employee.Name,
                Department = employee.Department.ToString(),
                Position = employee.Position.ToString(),
                IsApproved = user.IsApproved,
                Roles = roles
            };
        }
    }
}
