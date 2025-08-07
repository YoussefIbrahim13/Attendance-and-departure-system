using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace AttendanceSystem.Auth.API.Services
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;
        private readonly HttpClient _httpClient;

        public JwtAuthenticationStateProvider(
            ILocalStorageService localStorage,
            ISessionStorageService sessionStorage,
            HttpClient httpClient)
        {
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                // First try to get token from session storage (more secure)
                var token = await _sessionStorage.GetItemAsync<string>("authToken");

                // If not found in session, check localStorage (persistent)
                if (string.IsNullOrWhiteSpace(token))
                {
                    token = await _localStorage.GetItemAsync<string>("authToken");
                }

                if (string.IsNullOrWhiteSpace(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                // Set the default authorization header
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                return CreateAuthenticationState(token);
            }
            catch
            {
                // If any error occurs, return anonymous user
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyUserAuthentication(string token)
        {
            var authState = CreateAuthenticationState(token);
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public Task NotifyUserLogout()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
            return Task.CompletedTask;
        }

        private AuthenticationState CreateAuthenticationState(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var identity = new ClaimsIdentity(jwtToken.Claims, "jwt");
                var user = new ClaimsPrincipal(identity);
                return new AuthenticationState(user);
            }
            catch
            {
                // If token is invalid, treat as anonymous
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public async Task InitializeAsync()
        {
            // This will trigger authentication state check
            await GetAuthenticationStateAsync();
        }
    }
}