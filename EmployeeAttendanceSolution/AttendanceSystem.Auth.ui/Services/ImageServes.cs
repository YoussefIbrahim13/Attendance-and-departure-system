using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AttendanceSystem.Auth.ui.Services
{
    public class ImageServes
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;

        public ImageServes(HttpClient httpClient, ILocalStorageService localStorage)
        {
            _httpClient = httpClient; // This should be the ImportAPI client
            _localStorage = localStorage;
        }

        public async Task<string?> UploadProfileImageAsync(string employeeCode, IBrowserFile file)
        {
            try
            {
                if (file == null) return null;

                var token = await _localStorage.GetItemAsync<string>("authToken");

                using var content = new MultipartFormDataContent();

                var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024);
                content.Add(new StreamContent(stream), "file", file.Name);

                // Set authorization header
                if (!string.IsNullOrEmpty(token))
                {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                // Use relative path - HttpClient should have BaseAddress = https://localhost:7002/
                var response = await _httpClient.PostAsync($"api/Attendance/upload-profile-image/{employeeCode}", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Upload failed: {error}");
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<ProfileImageResult>();
                return result?.ImageUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading image: {ex.Message}");
                return null;
            }
            finally
            {
                // Clear authorization header
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }
    }

    public class ProfileImageResult
    {
        public string? ImageUrl { get; set; }
        public string? Message { get; set; }
    }
}