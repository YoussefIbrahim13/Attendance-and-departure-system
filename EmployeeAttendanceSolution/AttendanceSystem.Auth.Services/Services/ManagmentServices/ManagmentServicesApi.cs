using EmployeesModels.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AttendanceSystem.Auth.API.Services.Services.ManagmentServices
{
    public class ManagmentServicesApi : IManagmentServicesApi
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ManagmentServicesApi(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
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

        public async Task<OperationResult<List<UserResponseDto>>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                var userDtos = new List<UserResponseDto>();
                foreach (var u in users)
                {
                    userDtos.Add(new UserResponseDto
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                        Email = u.Email,
                        Name = u.Name,
                        Department = u.Department,
                        Position = u.Position,
                        IsApproved = u.IsApproved,
                        IsLockedByAdmin = u.IsLockedByAdmin,   // 🔴 Added
                        Roles = await _userManager.GetRolesAsync(u)
                    });
                }

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


        public async Task<UserResult> AddApplicationUserAsync(UserCreateDto dto, string roleName)
        {
            if (!new EmailAddressAttribute().IsValid(dto.Email))
                return new UserResult { Errors = new[] { new IdentityError { Description = "Invalid email format" } } };
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
                IsLockedByAdmin = user.IsLockedByAdmin,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task<UserResult> UpdateApplicationUserAsync(string id, UserUpdateDto dto)
        {
            // 1. Find user
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return new UserResult { Errors = new[] { new IdentityError { Description = "User not found" } } };

            // 2. Email validation and update
            if (!string.IsNullOrEmpty(dto.Email))
            {
                // Check if email is changing
                if (!string.Equals(dto.Email, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    // Validate email format
                    var emailValidator = new EmailAddressAttribute();
                    if (!emailValidator.IsValid(dto.Email))
                        return new UserResult { Errors = new[] { new IdentityError { Description = "Invalid email format" } } };

                    // Check for duplicate email
                    var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                    if (existingUser != null && existingUser.Id != user.Id)
                        return new UserResult { Errors = new[] { new IdentityError { Description = "Email is already in use" } } };

                    // Update email-related fields
                    user.Email = dto.Email;
                    user.UserName = dto.Email;
                    user.NormalizedEmail = _userManager.NormalizeEmail(dto.Email);
                    user.NormalizedUserName = _userManager.NormalizeEmail(dto.Email);
                }
            }

            // 3. Update other properties
            user.Name = dto.Name ?? user.Name;
            user.Department = dto.Department ?? user.Department;
            user.Position = dto.Position ?? user.Position;
            user.IsApproved = dto.IsApproved; // Add approval status update

            // 4. Save changes
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
                return new UserResult { Errors = updateResult.Errors };

            // 5. Return success result
            return new UserResult
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Department = user.Department,
                Position = user.Position,
                IsApproved = user.IsApproved,
                IsLockedByAdmin = user.IsLockedByAdmin,
                Roles = await _userManager.GetRolesAsync(user)
            };
        }

        public async Task<OperationResult> DeleteApplicationUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = $"User with ID {userId} not found"
                    };
                }

                // Delete the user (ignore the result)
                await _userManager.DeleteAsync(user);

                return new OperationResult
                {
                    Success = true,
                    Message = "User deleted successfully"
                };
            }
            catch (Exception ex)
            {
                // Log the exception (use ILogger in production)
                Console.WriteLine($"Error deleting user {userId}: {ex}");

                return new OperationResult
                {
                    Success = false,
                    Message = $"An unexpected error occurred while deleting the user: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult> ApproveUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new OperationResult { Success = false, Message = $"User with ID {userId} not found" };

            user.IsApproved = true;
            await _userManager.UpdateAsync(user); // Ignore the result (log if needed)

            return new OperationResult { Success = true, Message = "User approved successfully" };
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
        public async Task<OperationResult> ChangePasswordAsync(string userId, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = $"User with ID {userId} not found"
                    };
                }

                // Optionally: Prevent using the same password
                if (await _userManager.CheckPasswordAsync(user, newPassword))
                {
                    return new OperationResult
                    {
                        Success = false,
                        Message = "New password cannot be the same as current password"
                    };
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

                if (result.Succeeded)
                {
                    return new OperationResult
                    {
                        Success = true,
                        Message = "Password changed successfully"
                    };
                }

                return new OperationResult
                {
                    Success = false,
                    Message = string.Join(", ", result.Errors.Select(e => e.Description))
                };
            }
            catch (Exception ex)
            {
                // Log the exception here if you have logging configured
                return new OperationResult
                {
                    Success = false,
                    Message = "An error occurred while changing the password"
                };
            }
        }

        public async Task<OperationResult> UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
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
