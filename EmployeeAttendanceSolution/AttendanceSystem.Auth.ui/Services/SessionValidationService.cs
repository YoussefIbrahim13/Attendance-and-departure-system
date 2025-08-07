using AttendanceSystem.Auth.API.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace AttendanceSystem.Auth.ui.Services
{
    public class SessionValidationService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;
        private readonly AuthenticationStateProvider _authProvider;

        public SessionValidationService(
            ILocalStorageService localStorage,
            ISessionStorageService sessionStorage,
            AuthenticationStateProvider authProvider)
        {
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
            _authProvider = authProvider;
        }

        public async Task ValidateSession()
        {
            var sessionToken = await _sessionStorage.GetItemAsync<string>("sessionToken");
            var persistentToken = await _localStorage.GetItemAsync<string>("authToken");

            if (string.IsNullOrEmpty(sessionToken))
            {
                // No active session - clear persistent token if exists
                if (!string.IsNullOrEmpty(persistentToken))
                {
                    await _localStorage.RemoveItemAsync("authToken");
                    ((JwtAuthenticationStateProvider)_authProvider).NotifyUserLogout();
                }
            }
        }
    }
}
