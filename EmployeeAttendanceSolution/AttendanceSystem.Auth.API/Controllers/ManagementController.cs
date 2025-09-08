using AttendanceSystem.Auth.Services.Features.Employees.Queries.GetAllEmployees;
using AttendanceSystem.Auth.Services.Features.Roles.Commands.CreateRole;
using AttendanceSystem.Auth.Services.Features.Roles.Queries.GetAllRoles;
using AttendanceSystem.Auth.Services.Features.Roles.Queries.GetRoleById;
using AttendanceSystem.Auth.Services.Features.Users.Commands.AddUser;
using AttendanceSystem.Auth.Services.Features.Users.Commands.ApproveUser;
using AttendanceSystem.Auth.Services.Features.Users.Commands.ChangePassword;
using AttendanceSystem.Auth.Services.Features.Users.Commands.DeleteUser;
using AttendanceSystem.Auth.Services.Features.Users.Commands.UnlockUser;
using AttendanceSystem.Auth.Services.Features.Users.Commands.UpdateUser;
using AttendanceSystem.Auth.Services.Features.Users.Queries.GetAllUsers;
using AttendanceSystem.Auth.Services.Features.Users.Queries.GetPendingUsers;
using AttendanceSystem.Auth.Services.Features.Users.Queries.GetUserByEmployeeCode;
using AttendanceSystem.Auth.Services.Features.Users.Queries.GetUserById;
using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystem.Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CreateRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var result = await _mediator.Send(new CreateRoleCommand(roleName));
            return result.Success
                   ? CreatedAtAction(nameof(GetRole), new { id = result.Id }, result)
                   : BadRequest(result.Errors);
        }

        [HttpGet("GetRole/{id}")]
        public async Task<IActionResult> GetRole(string id)
        {
            var result = await _mediator.Send(new GetRoleByIdQuery(id));
            return result.Success ? Ok(result) : NotFound();
        }

        [HttpGet("GetAllRoles")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _mediator.Send(new GetAllRolesQuery());
            return Ok(roles);
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _mediator.Send(new GetAllUsersQuery());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("AddApplicationUser")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddApplicationUser(
     [FromBody] UserCreateDto dto,
     [FromQuery] string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
            {
                return BadRequest("Role name is required");
            }

            var result = await _mediator.Send(new AddUserCommand(dto, roleName));

            return result.Success
                ? CreatedAtAction(nameof(GetApplicationUser), new { id = result.Id }, result)
                : BadRequest(result.Errors);
        }

        [HttpGet("GetApplicationUser/{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetApplicationUser(string id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));
            return result.Success ? Ok(result) : NotFound();
        }

        [HttpPut("UpdateApplicationUser/{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateApplicationUser(string id, [FromBody] UserUpdateDto dto)
        {
            var result = await _mediator.Send(new UpdateUserCommand(id, dto));
            return result.Success ? Ok(result) : BadRequest(result.Errors);
        }

        [HttpDelete("DeleteApplicationUser/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> DeleteApplicationUser(string userId)
        {
            var result = await _mediator.Send(new DeleteUserCommand(userId));
            return Ok(new { result.Success, result.Message });
        }

        [HttpGet("GetUserByEmployeeCode/{employeeCode}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetUserByEmployeeCode([FromRoute] string employeeCode)
        {
            var query = new GetUserByEmployeeCodeQuery { EmployeeCode = employeeCode };

            var result = await _mediator.Send(query);
            return result.Success ? Ok(result) : NotFound(result.Errors);
        }

        [HttpGet("GetAllEmployees")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetAllEmployees()
        {
            var result = await _mediator.Send(new GetAllEmployeesQuery());
            return Ok(result);
        }

        [HttpPost("approve/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> ApproveUser(string userId)
        {
            var result = await _mediator.Send(new ApproveUserCommand(userId));
            return Ok(new { result.Success, result.Message });
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> GetPendingUsers()
        {
            var users = await _mediator.Send(new GetPendingUsersQuery());
            return Ok(users);
        }

        [HttpPut("ChangePassword/{userId}")]
        [Authorize(Roles = "Manager,Admin,User")]
        public async Task<IActionResult> ChangePassword(string userId, [FromBody] string newPassword)
        {
            if (string.IsNullOrWhiteSpace(newPassword))
                return BadRequest("New password is required");

            var result = await _mediator.Send(new ChangePasswordCommand(userId, newPassword));
            return Ok(new { result.Success, result.Message });
        }

        [HttpPut("UnlockUser/{userId}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UnlockUser(string userId)
        {
            var result = await _mediator.Send(new UnlockUserCommand(userId));
            return Ok(new { result.Success, result.Message });
        }

    }
}
