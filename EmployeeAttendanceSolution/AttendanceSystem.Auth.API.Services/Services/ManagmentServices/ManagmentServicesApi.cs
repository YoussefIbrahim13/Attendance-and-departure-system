using EmployeesModels.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Auth.API.Services.Services.ManagmentServices
{
    public class ManagmentServicesApi : IManagmentServicesApi
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ManagmentServicesApi(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<RoleResult> CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return new RoleResult { Errors = new[] { new IdentityError { Description = "Role name cannot be empty" } } };

            if (await _roleManager.RoleExistsAsync(roleName))
                return new RoleResult { Errors = new[] { new IdentityError { Description = $"Role '{roleName}' already exists" } } };

            if (!Enum.TryParse<Roles>(roleName, true, out var roleType))
                return new RoleResult { Errors = new[] { new IdentityError { Description = $"Invalid role. Valid roles are: {string.Join(", ", Enum.GetNames(typeof(Roles)))}" } } };

            var role = new ApplicationRole
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper(),
                RoleType = roleType
            };

            var result = await _roleManager.CreateAsync(role);

            return result.Succeeded
                ? new RoleResult { Id = role.Id, Name = role.Name, RoleType = role.RoleType }
                : new RoleResult { Errors = result.Errors };
        }

        public async Task<RoleResult> GetRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            return role == null
                ? new RoleResult { Errors = new[] { new IdentityError { Description = "Role not found" } } }
                : new RoleResult { Id = role.Id, Name = role.Name, RoleType = role.RoleType };
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        }

        public async Task<UserResult> AddApplicationUserAsync(UserCreateDto dto, string roleName)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Department = dto.Department,
                Position = dto.Position,
                IsApproved = true
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return new UserResult { Errors = result.Errors };

            if (!await _roleManager.RoleExistsAsync(roleName))
                return new UserResult { Errors = new[] { new IdentityError { Description = "Role does not exist" } } };

            await _userManager.AddToRoleAsync(user, roleName);

            return new UserResult
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Department = user.Department,
                Position = user.Position,
                IsApproved = user.IsApproved,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task<UserResult> GetApplicationUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new UserResult { Errors = new[] { new IdentityError { Description = "User not found" } } };

            return new UserResult
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Department = user.Department,
                Position = user.Position,
                IsApproved = user.IsApproved,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task<UserResult> UpdateApplicationUserAsync(string id, UserUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new UserResult { Errors = new[] { new IdentityError { Description = "User not found" } } };

            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = user.UserName = dto.Email;

            user.Name = dto.Name ?? user.Name;
            user.Department = dto.Department ?? user.Department;
            user.Position = dto.Position ?? user.Position;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new UserResult { Errors = result.Errors };

            return new UserResult
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Department = user.Department,
                Position = user.Position,
                IsApproved = user.IsApproved,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task<OperationResult> DeleteApplicationUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new OperationResult { Success = false, Message = $"User with ID {userId} not found" };

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded
                ? new OperationResult { Success = true, Message = "User deleted successfully" }
                : new OperationResult { Success = false, Errors = result.Errors };
        }

        public async Task<OperationResult> ApproveUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new OperationResult { Success = false, Message = $"User with ID {userId} not found" };

            user.IsApproved = true;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? new OperationResult { Success = true, Message = "User approved successfully" }
                : new OperationResult { Success = false, Errors = result.Errors };
        }

        public async Task<IEnumerable<UserResponseDto>> GetPendingUsersAsync()
        {
            return await _userManager.Users
                .Where(u => !u.IsApproved)
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    Name = u.Name,
                    Department = u.Department,
                    Position = u.Position
                })
                .ToListAsync();
        }
    }

}
