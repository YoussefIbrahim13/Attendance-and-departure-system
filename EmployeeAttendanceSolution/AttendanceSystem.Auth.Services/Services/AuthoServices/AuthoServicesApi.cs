using EmployeesModels.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Domain.Entities;


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

            if (user == null)
                return new AuthResult { ErrorMessage = "Invalid credentials" };

            // Check if admin has locked the user
            if (user.IsLockedByAdmin)
                return new AuthResult { ErrorMessage = "Account locked by admin. Contact support." };

            // Verify password
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var failedCount = await _userManager.GetAccessFailedCountAsync(user);
                await _userManager.AccessFailedAsync(user);

                if (failedCount + 1 >= 5) // because AccessFailedAsync increments after
                {
                    user.IsLockedByAdmin = true;
                    await _userManager.UpdateAsync(user);
                }

                return new AuthResult { ErrorMessage = "Invalid credentials" };
            }

            // Reset failed count if login successful
            await _userManager.ResetAccessFailedCountAsync(user);

            if (!user.IsApproved)
                return new AuthResult { ErrorMessage = "Account not approved yet" };

            // Build claims for JWT
            var authClaims = new List<Claim>
    {
        new(ClaimTypes.Name, user.Name),
        new(ClaimTypes.Email, user.Email),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new("IsApproved", user.IsApproved.ToString()),
        new(ClaimTypes.NameIdentifier, user.Id)
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

                // Load user with Employee
                var user = await _userManager.Users
                    .Include(u => u.Employee)
                    .FirstOrDefaultAsync(u => u.Email == email);

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
                    PhoneNumber=user.Employee?.Phone,
                    Code = user.Employee?.Code,
                    ProfileImagePath = user.Employee?.ProfileImagePath,
                    Department = user.Employee?.Department.ToString(),
                    Position = user.Employee?.Position.ToString(),
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
