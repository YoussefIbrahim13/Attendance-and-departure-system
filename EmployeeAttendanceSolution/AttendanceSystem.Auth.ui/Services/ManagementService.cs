using Blazored.LocalStorage;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.Net.Http.Json;

namespace AttendanceSystem.Auth.ui.Services
{
    public class ManagementService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ManagementService(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
        }

        private async Task<HttpClient> GetAuthorizedClient()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            return _httpClient;
        }

        public async Task<RoleResult> CreateRoleAsync(string roleName)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PostAsJsonAsync("api/Management/CreateRole", roleName);
            return await HandleResponse<RoleResult>(response);
        }

        public async Task<RoleResult> GetRoleAsync(string id)
        {
            var client = await GetAuthorizedClient();
            return await client.GetFromJsonAsync<RoleResult>($"api/Management/GetRole/{id}");
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            var client = await GetAuthorizedClient();
            return await client.GetFromJsonAsync<IEnumerable<string>>("api/Management/GetAllRoles");
        }

        public async Task<OperationResult<List<UserResponseDto>>> GetAllUsersAsync()
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync("api/Management/GetAllUsers");
            return await HandleResponse<OperationResult<List<UserResponseDto>>>(response);
        }

        public async Task<UserResult> AddApplicationUserAsync(UserCreateDto dto, string roleName)
        {
            try
            {
                // URL encode the roleName and create the full URL
                var encodedRoleName = Uri.EscapeDataString(roleName);
                var url = $"api/Management/AddApplicationUser?roleName={encodedRoleName}";

                Console.WriteLine($"Calling API: {url}");
                Console.WriteLine($"With data: Name={dto.Name}, Email={dto.Email}, EmployeeCode={dto.EmployeeCode}");

                var response = await _httpClient.PostAsJsonAsync(url, dto);

                return await HandleResponse<UserResult>(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddApplicationUserAsync: {ex.Message}");
                return new UserResult { Errors = new[] { new IdentityError { Description = ex.Message } } };
            }
        }

        public async Task<UserResult> GetApplicationUserAsync(string id)
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync($"api/Management/GetApplicationUser/{id}");
            return await HandleResponse<UserResult>(response);
        }

        public async Task<UserResult> UpdateApplicationUserAsync(string id, UserUpdateDto dto)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Management/UpdateApplicationUser/{id}", dto);
            return await HandleResponse<UserResult>(response);
        }

        public async Task<OperationResult> DeleteApplicationUserAsync(string userId)
        {
            var client = await GetAuthorizedClient();
            var response = await client.DeleteAsync($"api/Management/DeleteApplicationUser/{userId}");
            return await HandleResponse<OperationResult>(response);
        }
        public async Task<UserResult> GetUserByEmployeeCodeAsync(string employeeCode)
        {
           
            
            try
            {
                var client = await GetAuthorizedClient();
                var response = await client.GetAsync($"api/Management/GetUserByEmployeeCode/{employeeCode}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // 404 means no user found - return null instead of throwing
                    return null;
                }

                response.EnsureSuccessStatusCode();
                return await HandleResponse<UserResult>(response);
               // return await response.Content.ReadFromJsonAsync<UserResult>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error checking user by employee code: {ex.Message}");
                throw;
            }
        }
       

        /// //////////////////////////////////////
        public async Task<List<EmployeeResult>> GetAllEmployeesAsync()
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync("api/Management/GetAllEmployees");
            return await HandleResponse<List<EmployeeResult>>(response)?? new List<EmployeeResult>();
        }


        /// //////////////////////////////////////

        public async Task<OperationResult> ApproveUserAsync(string userId)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PostAsync($"api/Management/approve/{userId}", null);
            return await HandleResponse<OperationResult>(response);
        }

        public async Task<IEnumerable<UserResponseDto>> GetPendingUsersAsync()
        {
            var client = await GetAuthorizedClient();
            return await client.GetFromJsonAsync<IEnumerable<UserResponseDto>>("api/Management/pending");
        }
        public async Task<OperationResult> ChangePasswordAsync(string userId, string newPassword)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/Management/ChangePassword/{userId}", newPassword);
            return await HandleResponse<OperationResult>(response);
        }
        public async Task<OperationResult> UnlockUserAsync(string userId)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PutAsync($"api/Management/UnlockUser/{userId}", null);
            return await HandleResponse<OperationResult>(response);
        }

        private async Task<T> HandleResponse<T>(HttpResponseMessage response) where T : class
        {
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>();
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"API request failed with status code {response.StatusCode}: {errorContent}");
        }

    }
}
