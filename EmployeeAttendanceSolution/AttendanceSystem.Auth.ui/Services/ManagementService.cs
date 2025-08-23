using Blazored.LocalStorage;
using EmployeesModels.Shared;
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
            var client = await GetAuthorizedClient();
            var response = await client.PostAsJsonAsync($"api/Management/AddApplicationUser?roleName={roleName}", dto);
            return await HandleResponse<UserResult>(response);
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
