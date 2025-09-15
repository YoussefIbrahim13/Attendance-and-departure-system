using AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.ApproveVacationRequest;
using AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.CreateVacationRequest;
using AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.DeleteVacationRequest;
using AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.RejectVacationRequest;
using AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.UpdateVacationRequest;
using AttendanceSystem.Auth.Services.Features.VacationRequests.Queries.GetAllVacationRequests;
using AttendanceSystem.Auth.Services.Features.VacationRequests.Queries.GetVacationRequestById;
using AttendanceSystem.Auth.Services.Features.VacationRequests.Queries.GetVacationRequestsByUserId;
using EmployeesModels.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystem.Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacationRequestsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public VacationRequestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("CreateVacationRequest")]
        public async Task<IActionResult> CreateVacationRequest([FromBody] CreateVacationRequestDto vacationRequest)
        {
            // If UserId not provided, take it from authenticated user
            if (string.IsNullOrEmpty(vacationRequest.UserId))
                vacationRequest.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(vacationRequest.UserId))
                return Unauthorized(new OperationResult { Success = false, Message = "User not authenticated" });

            var result = await _mediator.Send(new CreateVacationRequestCommand(vacationRequest.UserId, vacationRequest));

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllVacationRequestsQuery());
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetVacationRequestByIdQuery(id));
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("GetByUser/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var result = await _mediator.Send(new GetVacationRequestsByUserIdQuery(userId));
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpGet("GetMyRequests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new OperationResult { Success = false, Message = "User not authenticated" });

            var result = await _mediator.Send(new GetVacationRequestsByUserIdQuery(userId));
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateVacationRequestDto vacationRequest)
        {
            var result = await _mediator.Send(new UpdateVacationRequestCommand(id, vacationRequest));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mediator.Send(new DeleteVacationRequestCommand(id));
            return result.Success ? Ok(result) : NotFound(result);
        }

        [HttpPut("Approve/{id}")]
        public async Task<IActionResult> Approve(string id)
        {
            var approvedBy = User.Identity?.Name ??
                     User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                     "System";

            var result = await _mediator.Send(new ApproveVacationRequestCommand(id, approvedBy));
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("Reject/{id}")]
        public async Task<IActionResult> Reject(string id)
        {
            var result = await _mediator.Send(new RejectVacationRequestCommand(id));
            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
