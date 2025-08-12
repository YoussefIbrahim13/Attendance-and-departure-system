using EmployeesModels.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


namespace AttendanceSystem.Auth.API.Services.Services.AuthoServices
{
    public class AuthoServicesApi :IAuthoServicesApi
    {
        readonly UserManager<ApplicationUser> _userManager;
        readonly IConfiguration _configuration;

        public AuthoServicesApi(UserManager<ApplicationUser> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }
        public async Task<AuthResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return new AuthResult { ErrorMessage = "Invalid credentials" };

            if (!user.IsApproved)
                return new AuthResult { ErrorMessage = "Account not approved yet" };

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name),
                new(ClaimTypes.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("IsApproved", user.IsApproved.ToString()),
                new (ClaimTypes.NameIdentifier, user.Id)
            };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = GenerateJwtToken(authClaims);

            return new AuthResult
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                User = new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Roles = userRoles
                }
            };
        }

       
        public AuthResult Logout()
        {
            return new AuthResult
            {
                ErrorMessage = null // Success
            };
        }
        public JwtSecurityToken GenerateJwtToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(
                 Encoding.UTF8.GetBytes(_configuration["JwtSettings:SecretKey"]));

            return new JwtSecurityToken(
                issuer: _configuration["JwtSettings:ValidIssuer"],
                audience: _configuration["JwtSettings:ValidAudience"],
                expires: DateTime.UtcNow.AddHours(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(
                    authSigningKey, SecurityAlgorithms.HmacSha256));
        }
        public async Task<UserInfo?> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            try
            {
                Console.WriteLine("🔍 GetCurrentUserAsync started");

                if (userPrincipal?.Identity?.IsAuthenticated != true)
                {
                    Console.WriteLine("❌ User not authenticated");
                    return null;
                }

                // Log all claims
                Console.WriteLine("📜 User Claims:");
                foreach (var claim in userPrincipal.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
                }

                var email = userPrincipal.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    Console.WriteLine("⚠ No email claim found");
                    return null;
                }

                Console.WriteLine($"🔎 Looking up user by email: {email}");

                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    Console.WriteLine($"❌ User with email {email} not found in database");
                    return null;
                }

                Console.WriteLine($"✅ User found: {user.UserName}, IsApproved: {user.IsApproved}");

                if (!user.IsApproved)
                {
                    Console.WriteLine("⚠ User not approved");
                    return null;
                }

                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"👤 User roles: {string.Join(", ", roles)}");

                return new UserInfo
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = user.Name,
                    Roles = roles
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Error in GetCurrentUserAsync: {ex}");
                return null;
            }
        }


    }
}
