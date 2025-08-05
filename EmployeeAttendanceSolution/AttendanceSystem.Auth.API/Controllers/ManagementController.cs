using AttendanceSystem.Auth.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystem.Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,Manager")]
    public class ManagementController : ControllerBase
    {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<ApplicationRole> _roleManager;

        public ManagementController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [HttpPost("roles")]
        [Authorize(Roles = "Admin")] // Typically only Admins should create roles
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(roleName))
                return BadRequest("Role name cannot be empty");

            // Check if role already exists
            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest($"Role '{roleName}' already exists");

            // Try to parse the enum (optional - only if you want to restrict to enum values)
            if (!Enum.TryParse<Roles>(roleName, true, out var roleType))
                return BadRequest($"Invalid role. Valid roles are: {string.Join(", ", Enum.GetNames(typeof(Roles)))}");

            var role = new ApplicationRole
            {
                Name = roleName,
                NormalizedName = roleName.ToUpper(), // Important for case-insensitive lookups
                RoleType = roleType
            };

            var result = await _roleManager.CreateAsync(role);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return CreatedAtAction(nameof(GetRole),
                new { id = role.Id },
                new
                {
                    role.Id,
                    role.Name,
                    role.RoleType
                });
        }

        [HttpGet("roles/{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            return Ok(new { role.Id, role.Name, role.RoleType });
        }

        [HttpGet("roles")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync();

            return Ok(roles);
        }

        [HttpPost("AddApplicationUser")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddApplicationUser(
                        [FromBody] UserCreateDto dto,
        [FromQuery] string roleName)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name,
                Department = dto.Department,
                Position = dto.Position,
                IsApproved = !User.IsInRole("Admin") // Admins bypass approval
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            if (!await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("Role does not exist");

            await _userManager.AddToRoleAsync(user, roleName);

            return CreatedAtAction(nameof(GetUser), new { id = user.Id },
                new UserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Department = user.Department,
                    Position = user.Position,
                    IsApproved = user.IsApproved,
                    Roles = await _userManager.GetRolesAsync(user)
                });
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Name = user.Name,
                Department = user.Department,
                Position = user.Position,
                IsApproved = user.IsApproved,
                Roles = await _userManager.GetRolesAsync(user)
            });
        }



        [HttpPut("UpdateApplicationUser/{userId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDto dto)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.Email))
                user.Email = user.UserName = dto.Email;

            user.Name = dto.Name ?? user.Name;
            user.Department = dto.Department ?? user.Department;
            user.Position = dto.Position ?? user.Position;

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? Ok(new UserResponseDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Department = user.Department,
                    Position = user.Position,
                    IsApproved = user.IsApproved
                })
                : BadRequest(result.Errors);
        }


        [HttpDelete("DeleteApplicationUser/{userId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound(new { Error = $"User with ID {userId} not found" });
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded
                ? Ok(new { Message = "Application User deleted successfully" })
                : BadRequest(result.Errors);
        }


        [HttpPost("approve/{userId}")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound(new { Error = $"User with ID {userId} not found" });

            user.IsApproved = true;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? Ok(new { Message = "User approved successfully" })
                : BadRequest(result.Errors);
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _userManager.Users
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

            return Ok(users);
        }



    }
}