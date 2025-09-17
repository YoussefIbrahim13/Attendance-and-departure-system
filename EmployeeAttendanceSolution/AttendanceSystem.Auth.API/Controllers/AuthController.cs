using AttendanceSystem.Auth.API.Services.Services.AuthoServices;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AttendanceSystem.Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthoServicesApi _authService;

        public AuthController(IAuthoServicesApi authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _authService.Login(model);

            if (!result.IsSuccess)
                return Unauthorized(result.ErrorMessage);

            return Ok(new
            {
                Token = result.Token,
                Expiration = result.Expiration,
                User = result.User
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var result = _authService.Logout();
            return Ok(new { Message = "Logout successful" });
        }

        [HttpGet("currentuser")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var userInfo = await _authService.GetCurrentUserAsync(User);

                if (userInfo == null)
                {
                    return Unauthorized(new
                    {
                        Message = "User not found or not authorized",
                        Solution = "Please ensure your account exists, is approved, and you are logged in"
                    });
                }

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 API CurrentUser error: {ex}");
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing your request",
                    Detail = ex.Message
                });
            }
        }


    }
}
