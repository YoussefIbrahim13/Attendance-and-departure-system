using AttendanceSystem.Auth.API.Services.Services.AuthoServices;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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

    }
}
