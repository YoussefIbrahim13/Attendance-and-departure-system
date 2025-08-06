using EmployeesModels.Shared;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AttendanceSystem.Auth.API.Services.Services.AuthoServices
{
    public interface IAuthoServicesApi
    {
        Task<AuthResult> Login(LoginModel model);
        AuthResult Logout();
        JwtSecurityToken GenerateJwtToken(List<Claim> authClaims);

    }
}
