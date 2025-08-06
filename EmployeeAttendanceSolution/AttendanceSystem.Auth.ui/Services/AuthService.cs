using Blazored.LocalStorage;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

    namespace AttendanceSystem.Auth.API.Services
    {
        public class AuthService
        {
            private readonly HttpClient _http;
            private readonly ILocalStorageService _localStorage;
            private readonly JwtAuthenticationStateProvider _authProvider;

            public AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authProvider)
            {
                _http = http;
                _localStorage = localStorage;
                _authProvider = (JwtAuthenticationStateProvider)authProvider;
            }

            public async Task<bool> Login(LoginModel model)
            {
                var response = await _http.PostAsJsonAsync("api/auth/login", model);

                if (!response.IsSuccessStatusCode)
                    return false;

                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

                await _localStorage.SetItemAsync("authToken", result.Token);
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);
                _authProvider.NotifyUserAuthentication(result.Token);

                return true;
            }

            public async Task Logout()
            {
                await _localStorage.RemoveItemAsync("authToken");
                _http.DefaultRequestHeaders.Authorization = null;
                _authProvider.NotifyUserLogout();
            }

            public async Task Initialize()
            {
                var token = await _localStorage.GetItemAsync<string>("authToken");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    _authProvider.NotifyUserAuthentication(token);
                }
            }
        }

       

       
    }

