using EmployeesModels.Shared;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Domain.Entities;
using Domain.Enums;

namespace AttendanceSystem.ImportFile.ui.Services
{



    public class BaseHTTPService
    {
        private readonly HttpClient _http;
        private readonly ILocalStorageService _localStorage;
        private List<AttendanceRecord> _records = new();
        public BaseHTTPService(HttpClient http, ILocalStorageService localStorage)
        {
            _http = http;
            _localStorage = localStorage;
        }

        private async Task AddAuthHeaderAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            _http.DefaultRequestHeaders.Authorization = null;
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }


       
    

        // Update employeeProfile
        public async Task UpdateEmployeeAsync(EmployeeDto employee)
        {
            await AddAuthHeaderAsync();
            await _http.PutAsJsonAsync("api/Attendance/update-employee", employee);
        }

       
       

    }

    public class EmployeeDto
    {
        public Guid Id { get; set; } // Employee unique identifier
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DepartmentEnum Department { get; set; }
    public PositionEnum Position { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string? ProfileImagePath { get; set; }
    }


}
