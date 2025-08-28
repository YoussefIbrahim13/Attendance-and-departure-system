using AttendanceSystem.Auth.Services.Services.VacationRequestServices;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AttendanceSystem.Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacationRequestsController : ControllerBase
    {
        private readonly IVacationRequestServices _vacationRequestService;

        public VacationRequestsController(IVacationRequestServices vacationRequestService)
        {
            _vacationRequestService = vacationRequestService;
        }

        [HttpPost("CreateVacationRequest")]
        public async Task<IActionResult> CreateVacationRequest([FromBody] CreateVacationRequestDto vacationRequest)
        {



            // Get userId from authenticated user's claims
            if (string.IsNullOrEmpty(vacationRequest.UserId))
                vacationRequest.UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(vacationRequest.UserId))
            {
                return Unauthorized(new OperationResult { Success = false, Message = "User not authenticated" });
            }

            var result = await _vacationRequestService.CreateVacationRequestAsync(vacationRequest.UserId, vacationRequest);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _vacationRequestService.GetAllVacationRequestsAsync();

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _vacationRequestService.GetVacationRequestByIdAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetByUser/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var result = await _vacationRequestService.GetVacationRequestsByUserIdAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpGet("GetMyRequests")]
        public async Task<IActionResult> GetMyRequests()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new OperationResult { Success = false, Message = "User not authenticated" });
            }

            var result = await _vacationRequestService.GetVacationRequestsByUserIdAsync(userId);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateVacationRequestDto vacationRequest)
        {
            var result = await _vacationRequestService.UpdateVacationRequestAsync(id, vacationRequest);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _vacationRequestService.DeleteVacationRequestAsync(id);

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        [HttpPut("Approve/{id}")]
        public async Task<IActionResult> Approve(string id)
        {
            var result = await _vacationRequestService.ApproveVacationRequestAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("Reject/{id}")]
        public async Task<IActionResult> Reject(string id)
        {
            var result = await _vacationRequestService.RejectVacationRequestAsync(id);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}