using Blazored.LocalStorage;
using Blazored.SessionStorage;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AttendanceSystem.Auth.API.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;
        private readonly JwtAuthenticationStateProvider _authProvider;

        public AuthService(
            HttpClient http,
            ILocalStorageService localStorage,
            ISessionStorageService sessionStorage,
            AuthenticationStateProvider authProvider)
        {
            _http = http;
            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
            _authProvider = (JwtAuthenticationStateProvider)authProvider;
        }

        public async Task<LoginResult> Login(LoginModel model)
        {
            try
            {
                var response = await _http.PostAsJsonAsync("api/auth/login", model);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new LoginResult { Success = false, Error = $"Login failed: {errorContent}" };
                }

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                // Store token in localStorage
                await _localStorage.SetItemAsync("authToken", result.Token);

                // Set default authorization header
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", result.Token);

                // Notify authentication state change
                if (_authProvider is JwtAuthenticationStateProvider jwtProvider)
                {
                    jwtProvider.NotifyUserAuthentication(result.Token);
                }

                return new LoginResult { Success = true };
            }
            catch (Exception ex)
            {
                return new LoginResult { Success = false, Error = ex.Message };
            }
        }

        public async Task Logout()
        {
            // Remove token from storage
            await _localStorage.RemoveItemAsync("authToken");

            // Clear authorization header
            _http.DefaultRequestHeaders.Authorization = null;

            // Notify authentication state change
            if (_authProvider is JwtAuthenticationStateProvider jwtProvider)
            {
                jwtProvider.NotifyUserLogout();
            }
        }

        public async Task Initialize()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");

            if (!string.IsNullOrEmpty(token))
            {
                // Set the default authorization header
                _http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);

                // Notify authentication state change
                if (_authProvider is JwtAuthenticationStateProvider jwtProvider)
                {
                    jwtProvider.NotifyUserAuthentication(token);
                }
            }
        }

        public async Task<CurrentUserDto> GetCurrentUser()
        {
            try
            {
                // 1. Get token from storage (use localStorage only)
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (string.IsNullOrEmpty(token))
                {
                    token = await _sessionStorage.GetItemAsync<string>("authToken");
                    if (string.IsNullOrEmpty(token))
                        return null;
                }

                // 2. Manually attach the token to the request
                var request = new HttpRequestMessage(HttpMethod.Get, "api/auth/currentuser");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // 3. Send the request
                var response = await _http.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                    return null;

                // 4. Deserialize the response
                return await response.Content.ReadFromJsonAsync<CurrentUserDto>();
            }
            catch
            {
                return null;
            }
        }
    }
}