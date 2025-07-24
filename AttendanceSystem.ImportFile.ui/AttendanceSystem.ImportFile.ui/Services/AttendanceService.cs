using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace AttendanceSystem.ImportFile.ui.Services
{
    public class AttendanceService
    {
        private readonly HttpClient _http;
        public AttendanceService(HttpClient http)
        {
            _http = http;
        }

        // رفع ملف CSV
        public async Task<List<AttendanceRecord>?> UploadCsvAsync(MultipartFormDataContent content)
        {
            var response = await _http.PostAsync("Attendance/upload-csv", content);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<AttendanceRecord>>();
            return null;
        }

        // تعديل سجل مؤقت
        public async Task<bool> EditPendingAttendanceAsync(EditAttendanceDto dto)
        {
            var response = await _http.PutAsJsonAsync("Attendance/edit-pending", dto);
            return response.IsSuccessStatusCode;
        }

        // حفظ البيانات المؤقتة
        public async Task<bool> SaveAttendanceAsync()
        {
            var response = await _http.PostAsync("Attendance/save", null);
            return response.IsSuccessStatusCode;
        }
    }

    public class AttendanceRecord
    {
        public string EmployeeId { get; set; }
        public string Date { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string? Note { get; set; }
    }

    public class EditAttendanceDto
    {
        public string EmployeeId { get; set; }
        public string Date { get; set; }
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string? Note { get; set; }
    }
}
