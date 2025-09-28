using AttendanceSystem.Auth.API.Services;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace AttendanceSystem.Auth.ui.Services
{
    public class CurrentUserService
    {
        private readonly JwtAuthenticationStateProvider _authStateProvider;

        public CurrentUserService(JwtAuthenticationStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        public async Task<string?> GetUserIdAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<string?> GetEmailAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.FindFirst(ClaimTypes.Email)?.Value;
        }

        public async Task<string?> GetNameAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.FindFirst(ClaimTypes.Name)?.Value;
        }

        public async Task<List<string>> GetRolesAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            return authState.User.FindAll(ClaimTypes.Role)
                                 .Select(r => r.Value)
                                 .ToList();
        }
    }
}
