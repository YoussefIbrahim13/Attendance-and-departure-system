using AttendanceSystem.Auth.API.Services.Services.ManagmentServices;
using Azure.Core;
using EmployeesModels.Shared;
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
        private readonly IManagmentServicesApi _managementService;

        public ManagementController(IManagmentServicesApi managementService)
        {
            _managementService = managementService;
        }

        [HttpPost("CreateRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var result = await _managementService.CreateRoleAsync(roleName);
            return result.Success
                ? CreatedAtAction(nameof(GetRole), new { id = result.Id }, new { result.Id, result.Name, result.RoleType })
                : BadRequest(result.Errors);
        }

        [HttpGet("GetRole/{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            var result = await _managementService.GetRoleAsync(id);
            return result.Success
                ? Ok(new { result.Id, result.Name, result.RoleType })
                : NotFound();
        }

        [HttpGet("GetAllRoles")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _managementService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _managementService.GetAllUsersAsync();
            return result.Success
                ? Ok(result)  // Return the OperationResult directly
                : BadRequest(result);
        }

        [HttpPost("AddApplicationUser")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddApplicationUser(
            [FromBody] UserCreateDto dto,
            [FromQuery] string roleName)
        {
            var result = await _managementService.AddApplicationUserAsync(dto, roleName);
            return result.Success
                ? CreatedAtAction(nameof(GetApplicationUser), new { id = result.Id }, new UserResponseDto
                {
                    Id = result.Id,
                    UserName = result.UserName,
                    Email = result.Email,
                    Name = result.Name,
                    Department = result.Department,
                    Position = result.Position,
                    IsApproved = result.IsApproved,
                    Roles = result.Roles,
                    IsLockedByAdmin= false
                })
                : BadRequest(result.Errors);
        }

        [HttpGet("GetApplicationUser/{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetApplicationUser(string id)
        {
            var result = await _managementService.GetApplicationUserAsync(id);
            return result.Success
                ? Ok(new UserResponseDto
                {
                    Id = result.Id,
                    UserName = result.UserName,
                    Email = result.Email,
                    Name = result.Name,
                    Department = result.Department,
                    Position = result.Position,
                    IsApproved = result.IsApproved,
                    Roles = result.Roles
                })
                : NotFound();
        }

        [HttpPut("UpdateApplicationUser/{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateApplicationUser(string id, [FromBody] UserUpdateDto dto)
        {
            var result = await _managementService.UpdateApplicationUserAsync(id, dto);
            return result.Success
                ? Ok(new UserResponseDto
                {
                    Id = result.Id,
                    UserName = result.UserName,
                    Email = result.Email,
                    Name = result.Name,
                    Department = result.Department,
                    Position = result.Position,
                    IsApproved = result.IsApproved,
                    Roles = result.Roles
                })
                : BadRequest(result.Errors);
        }

        [HttpDelete("DeleteApplicationUser/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> DeleteApplicationUser(string userId)
        {
            var result = await _managementService.DeleteApplicationUserAsync(userId);

            // Return Ok if deletion was attempted (even if user didn't exist)
            return Ok(new { result.Success, result.Message });
        }

        [HttpPost("approve/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var result = await _managementService.ApproveUserAsync(userId);

            // Always return Ok if the user was found and updated
            return Ok(new { result.Success, result.Message });
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _managementService.GetPendingUsersAsync();
            return Ok(users);
        }
        [HttpPut("ChangePassword/{userId}")]
        [Authorize(Roles = "Manager,Admin,User")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] string newPassword)
        {

            if (string.IsNullOrWhiteSpace(newPassword))
            {
                return BadRequest("New password is required");
            }

            var result = await _managementService.ChangePasswordAsync(userId, newPassword);
            return Ok(new { result.Success, result.Message });
        }

        [HttpPut("UnlockUser/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            var result = await _managementService.UnlockUserAsync(userId);
            return Ok(new { result.Success, result.Message });
        }
    }

}
