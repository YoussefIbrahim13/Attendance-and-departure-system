using EmployeesModels.Shared;
using System.Net.Http.Json;

namespace AttendanceSystem.ImportFile.ui.Services
{



    public class AttendanceService: BaseHTTPService
    {
        private readonly HttpClient _http;
        private List<AttendanceRecord> _records = new();
        public AttendanceService(HttpClient http):base(http)
        {
            _http = http;
        }

        // رفع ملف CSV
        public  async Task<List<AttendanceRecord>?> UploadCsvAsync(MultipartFormDataContent content)
        {
          var response= await  Post<Task<List<AttendanceRecord>?>, MultipartFormDataContent>(content);
            //var response = await _http.PostAsync("Attendance/upload-csv", content);
            //if (response.IsSuccessStatusCode)
            //    return await response.Content.ReadFromJsonAsync<List<AttendanceRecord>>();
            //return null;
            return await response;
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
        // Get month view data
        public async Task<MonthViewDto?> GetMonthViewAsync(int year, int month)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<MonthViewDto>(
                    $"Attendance/month-view?year={year}&month={month}");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching month view data: {ex.Message}");
                return null;
            }
        }

        // ✅ Call API for a single day's attendance
        public async Task<List<DailyAttendanceDto>> GetDayViewAsync(DateTime date)
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<DailyAttendanceDto>>(
                    $"Attendance/day-view?date={date:yyyy-MM-dd}");

                return response ?? new List<DailyAttendanceDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching day view data: {ex.Message}");
                return new List<DailyAttendanceDto>();
            }
        }

        // Get year view data
        public async Task<YearViewDto?> GetYearViewAsync(int year)
        {
            try
            {
                return await _http.GetFromJsonAsync<YearViewDto>($"Attendance/year-view/{year}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error fetching year view: {ex.Message}");
                return null;
            }
        }

        // Get all employees (for Employees.razor)
        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
        {
            try
            {
                var response = await _http.GetFromJsonAsync<List<EmployeeDto>>("Attendance/employees");
                return response ?? new List<EmployeeDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching employees: {ex.Message}");
                return new List<EmployeeDto>();
            }
        }

        // Add new employee
        public async Task AddEmployeeAsync(EmployeeDto employee)
        {
            await _http.PostAsJsonAsync("Attendance/add-employee", employee);
        }

        // Delete employee
        public async Task DeleteEmployeeAsync(string id)
        {
            await _http.DeleteAsync($"Attendance/delete-employee/{id}");
        }

        // Update employee
        public async Task UpdateEmployeeAsync(EmployeeDto employee)
        {
            await _http.PutAsJsonAsync("Attendance/update-employee", employee);
        }

        // Legacy method - kept for backward compatibility
        public async Task<List<AttendanceDayStatus>> GetAttendanceByDateAsync(DateTime date)
        {
            try
            {
                var dayData = await GetDayViewAsync(date);
                return dayData.Select(d => new AttendanceDayStatus(
                    d.EmployeeId,
                    d.Date,
                    d.ActualStatus,
                    d.PlannedStatus, // Assuming PlannedStatus is part of DailyAttendanceDto
                    d.ApprovalStatus, // Assuming ApprovalStatus is part of DailyAttendanceDto
                    d.Note
                )).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching attendance data: {ex.Message}");
                return new List<AttendanceDayStatus>();
            }
        }

        // إضافة سجل حضور جديد still not implemented
        public async Task AddAttendanceAsync(AttendanceRecord record)
        {
            // This would typically call an API endpoint to add a new attendance record
            // For now, we'll just add it to the local list
            _records.Add(record);
            await Task.CompletedTask;
        }

        public async Task<bool> UpdateEmployeeAttendanceStatusAsync(EmployeeAttendanceRequest employeeAttendance)
        {
            try
            {
            var response = await _http.PutAsJsonAsync("Attendance/update-attendance-record", employeeAttendance);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error updating attendance: {response.StatusCode} - {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception updating attendance: {ex.Message}");
                return false;
            }
        }
        public async Task<bool> UpdateEmployeeAttendanceRecordAsync(AttendanceRecord EmployeeAttedanceRecord)
        {
            try
            {
                var response = await _http.PutAsJsonAsync("Attendance/update-attendance-record", EmployeeAttedanceRecord);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error updating attendance record: {response.StatusCode} - {errorContent}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception updating attendance record: {ex.Message}");
                return false;
            }

        }
        
        // جدولة حضور موظف لأيام محددة (Plan Attendance)
        public async Task<bool> PlanAttendanceAsync(string employeeId, List<DateTime> days, AttendanceStatus plannedStatus)
        {
            var dto = new PlanAttendanceDto
            {
                EmployeeId = employeeId,
                Dates = days,
                PlannedStatus = plannedStatus
            };
            var response = await _http.PostAsJsonAsync("Attendance/plan-attendance", dto);
            return response.IsSuccessStatusCode;
        }

    }

    //public class EmployeeDto
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //    public string Department { get; set; }
    //    public string Position { get; set; }
    //}

   
}
