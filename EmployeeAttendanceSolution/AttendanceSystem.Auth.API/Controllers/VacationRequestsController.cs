using AttendanceSystem.Auth.Services.Services.VacationRequestServices;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystem.Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VacationRequestsController : ControllerBase
    {
        readonly IVacationRequestServices _vacationRequestService;
        public VacationRequestsController(IVacationRequestServices vacationRequestService)
        {
            _vacationRequestService= vacationRequestService;
        }

        [HttpPost("CreateVacationRequest")]
        public async Task<IActionResult> CreateVacationRequest([FromBody] VacationRequest vacationRequest)
        {
            var result = await _vacationRequestService.CreateVacationRequestAsync(vacationRequest);
            //if (!result.Success)
            //    return BadRequest(result);
            //return Ok(result);
            return Ok(new { result.Success, result.Message });
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _vacationRequestService.GetAllVacationRequestsAsync();
            //if (!result.Success)
            //    return BadRequest(result);
            //return Ok(result);
            return Ok(new { result.Success, result.Message });
        }

        [HttpGet("GetById/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _vacationRequestService.GetVacationRequestByIdAsync(id);
            //if (!result.Success)
            //    return NotFound(result);
            //return Ok(result);
            return Ok(new { result.Success, result.Message });

        }

        [HttpGet("GetByUser/{userId}")]
        public async Task<IActionResult> GetByUser(string userId)
        {
            var result = await _vacationRequestService.GetVacationRequestsByUserIdAsync(userId);
            //if (!result.Success)
            //    return NotFound(result);
            //return Ok(result);
            return Ok(new { result.Success, result.Message });

        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] VacationRequest vacationRequest)
        {
            var result = await _vacationRequestService.UpdateVacationRequestAsync(id, vacationRequest);
            //if (!result.Success)
            //    return BadRequest(result);
            //return Ok(result);
            return Ok(new { result.Success, result.Message });

        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _vacationRequestService.DeleteVacationRequestAsync(id);
            //if (!result.Success)
            //    return NotFound(result);
            //return Ok(result);
            return Ok(new { result.Success, result.Message });

        }

        [HttpPut("Approve/{id}")]
        public async Task<IActionResult> Approve(string id)
        {
            var result = await _vacationRequestService.ApproveVacationRequestAsync(id);
            //if (!result.Success)
            //    return BadRequest(result);
            //return Ok(result);
            return Ok(new { result.Success, result.Message });

        }

        [HttpPut("Reject/{id}")]
        public async Task<IActionResult> Reject(string id)
        {
            var result = await _vacationRequestService.RejectVacationRequestAsync(id);
            //if (!result.Success)
            //    return BadRequest(result);
            //return Ok(result);
            return Ok(new { result.Success, result.Message });

        }
    }
}
