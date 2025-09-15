/*
using Blazored.LocalStorage;
using EmployeesModels.Shared;
using System.Net.Http.Json;

namespace AttendanceSystem.Auth.ui.Services
{
    public class VacationRequestServices
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public VacationRequestServices(HttpClient httpClient, ILocalStorageService localStorage)
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
        public async Task<OperationResult> CreateVacationRequestAsync(CreateVacationRequestDto request)
        {
            try
            {
                var client = await GetAuthorizedClient();
                var response = await client.PostAsJsonAsync("api/VacationRequests/CreateVacationRequest", request);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OperationResult>()
                        ?? new OperationResult { Success = false, Message = "No response from server." };
                }
                else
                {
                    // Try to read error details
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new OperationResult
                    {
                        Success = false,
                        Message = $"Error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"Network error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult<IEnumerable<VacationRequest>>> GetAllVacationRequestsAsync()
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync("api/VacationRequests/GetAll");
            return await response.Content.ReadFromJsonAsync<OperationResult<IEnumerable<VacationRequest>>>()
                   ?? new OperationResult<IEnumerable<VacationRequest>> { Success = false, Message = "No response from server." };
        }

        public async Task<OperationResult<VacationRequest>> GetVacationRequestByIdAsync(string id)
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync($"api/VacationRequests/GetById/{id}");
            return await response.Content.ReadFromJsonAsync<OperationResult<VacationRequest>>()
                   ?? new OperationResult<VacationRequest> { Success = false, Message = "No response from server." };
        }

        public async Task<OperationResult<IEnumerable<VacationRequest>>> GetVacationRequestsByUserIdAsync(string userId)
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync($"api/VacationRequests/GetByUser/{userId}");
            return await response.Content.ReadFromJsonAsync<OperationResult<IEnumerable<VacationRequest>>>()
                   ?? new OperationResult<IEnumerable<VacationRequest>> { Success = false, Message = "No response from server." };
        }

        public async Task<OperationResult<IEnumerable<VacationRequest>>> GetMyVacationRequestsAsync()
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync("api/VacationRequests/GetMyRequests");
            return await response.Content.ReadFromJsonAsync<OperationResult<IEnumerable<VacationRequest>>>()
                   ?? new OperationResult<IEnumerable<VacationRequest>> { Success = false, Message = "No response from server." };
        }
        public async Task<OperationResult> UpdateVacationRequestAsync(string id,UpdateVacationRequestDto  request)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/VacationRequests/Update/{id}", request);
            return await response.Content.ReadFromJsonAsync<OperationResult>()
                   ?? new OperationResult { Success = false, Message = "No response from server." };
        }

        public async Task<OperationResult> DeleteVacationRequestAsync(string id)
        {
            var client = await GetAuthorizedClient();
            var response = await client.DeleteAsync($"api/VacationRequests/Delete/{id}");
            return await response.Content.ReadFromJsonAsync<OperationResult>()
                   ?? new OperationResult { Success = false, Message = "No response from server." };
        }

        public async Task<OperationResult> ApproveVacationRequestAsync(string id)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PutAsync($"api/VacationRequests/Approve/{id}", null);
            return await response.Content.ReadFromJsonAsync<OperationResult>()
                   ?? new OperationResult { Success = false, Message = "No response from server." };
        }

        public async Task<OperationResult> RejectVacationRequestAsync(string id)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PutAsync($"api/VacationRequests/Reject/{id}", null);
            return await response.Content.ReadFromJsonAsync<OperationResult>()
                   ?? new OperationResult { Success = false, Message = "No response from server." };
        }

    }
}

*/
using Blazored.LocalStorage;
using EmployeesModels.Shared;
using System.Net.Http.Json;
using Domain.Entities;

namespace AttendanceSystem.Auth.ui.Services
{
    public class VacationRequestServices
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public VacationRequestServices(HttpClient httpClient, ILocalStorageService localStorage)
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

        // 🔹 Helper for consistent error handling
        private async Task<OperationResult<T>> HandleResponseAsync<T>(HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OperationResult<T>>()
                           ?? new OperationResult<T> { Success = false, Message = "No response from server." };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new OperationResult<T>
                    {
                        Success = false,
                        Message = $"Error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult<T>
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

        private async Task<OperationResult> HandleResponseAsync(HttpResponseMessage response)
        {
            try
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<OperationResult>()
                           ?? new OperationResult { Success = false, Message = "No response from server." };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new OperationResult
                    {
                        Success = false,
                        Message = $"Error: {response.StatusCode} - {errorContent}"
                    };
                }
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

        // ---------------- API Methods ----------------

        public async Task<OperationResult> CreateVacationRequestAsync(CreateVacationRequestDto request)
        {
            try
            {

                var client = await GetAuthorizedClient();
                var response = await client.PostAsJsonAsync("api/VacationRequests/CreateVacationRequest", request);
                return await HandleResponseAsync(response);
            }
            catch (HttpRequestException ex)
            {
                return new OperationResult { Success = false, Message = $"Network error: {ex.Message}" };
            }
        }

        public async Task<OperationResult<IEnumerable<VacationRequest>>> GetAllVacationRequestsAsync()
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync("api/VacationRequests/GetAll");
            return await HandleResponseAsync<IEnumerable<VacationRequest>>(response);
        }

        public async Task<OperationResult<VacationRequest>> GetVacationRequestByIdAsync(string id)
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync($"api/VacationRequests/GetById/{id}");
            return await HandleResponseAsync<VacationRequest>(response);
        }

        public async Task<OperationResult<IEnumerable<VacationRequest>>> GetVacationRequestsByUserIdAsync(string userId)
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync($"api/VacationRequests/GetByUser/{userId}");
            return await HandleResponseAsync<IEnumerable<VacationRequest>>(response);
        }

        public async Task<OperationResult<IEnumerable<VacationRequest>>> GetMyVacationRequestsAsync()
        {
            var client = await GetAuthorizedClient();
            var response = await client.GetAsync("api/VacationRequests/GetMyRequests");
            return await HandleResponseAsync<IEnumerable<VacationRequest>>(response);
        }

        public async Task<OperationResult> UpdateVacationRequestAsync(string id, UpdateVacationRequestDto request)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PutAsJsonAsync($"api/VacationRequests/Update/{id}", request);
            return await HandleResponseAsync(response);
        }

        public async Task<OperationResult> DeleteVacationRequestAsync(string id)
        {
            var client = await GetAuthorizedClient();
            var response = await client.DeleteAsync($"api/VacationRequests/Delete/{id}");
            return await HandleResponseAsync(response);
        }

        public async Task<OperationResult> ApproveVacationRequestAsync(string id)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PutAsync($"api/VacationRequests/Approve/{id}", null);
            return await HandleResponseAsync(response);
        }

        public async Task<OperationResult> RejectVacationRequestAsync(string id)
        {
            var client = await GetAuthorizedClient();
            var response = await client.PutAsync($"api/VacationRequests/Reject/{id}", null);
            return await HandleResponseAsync(response);
        }
    }
}

