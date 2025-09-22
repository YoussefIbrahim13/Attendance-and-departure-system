//using Blazored.LocalStorage;
//using Domain.Entities;
//using Domain.Enums;
//using EmployeesModels.Shared;
//using Microsoft.AspNetCore.Components.Forms;
//using System.Net.Http.Json;

//namespace AttendanceSystem.Auth.ui.Services
//{
//    public class AttendanceService : BaseHTTPService
//    {
//        private readonly HttpClient _http;
//        private List<AttendanceRecord> _records = new();
//        public AttendanceService(HttpClient http, ILocalStorageService localStorage) : base(http, localStorage)
//        {
//            _http = http;
//        }

//        // رفع ملف CSV
//        //public  async Task<List<AttendanceRecord>?> UploadCsvAsync(MultipartFormDataContent content)
//        //{
//        //  var response= await  Post<Task<List<AttendanceRecord>?>, MultipartFormDataContent>(content);
//        //    //var response = await _http.PostAsync("Attendance/upload-csv", content);
//        //    //if (response.IsSuccessStatusCode)
//        //    //    return await response.Content.ReadFromJsonAsync<List<AttendanceRecord>>();
//        //    //return null;
//        //    return await response;
//        //}
//        public async Task<List<AttendanceRecord>?> UploadCsvAsync(MultipartFormDataContent content)
//        {
//            var response = await _http.PostAsync("api/Attendance/upload-csv", content);
//            if (response.IsSuccessStatusCode)
//                return await response.Content.ReadFromJsonAsync<List<AttendanceRecord>>();
//            return null;
//        }

//        // تعديل سجل مؤقت
//        public async Task<bool> EditPendingAttendanceAsync(EditAttendanceDto dto)
//        {
//            var response = await _http.PutAsJsonAsync("api/Attendance/edit-pending", dto);
//            return response.IsSuccessStatusCode;
//        }

//        // حفظ البيانات المؤقتة
//        public async Task<bool> SaveAttendanceAsync(List<AttendanceRecord> pending)
//        {

//            var dtos = pending.Select(x => new SaveAttendanceRecordDto
//            {
//                Code = x.Code,
//                Date = x.Date,
//                CheckIn = x.CheckIn,
//                CheckOut = x.CheckOut,
//                ActualStatus = x.ActualStatus,
//                PlannedStatus = x.PlannedStatus,
//                ApprovalStatus = x.ApprovalStatus,
//                Note = x.Note
//            }).ToList();


//            var response = await _http.PostAsJsonAsync("api/Attendance/save", pending);
//            return response.IsSuccessStatusCode;
//        }

//        // Get month view data
//        public async Task<MonthViewDto?> GetMonthViewAsync(int year, int month)
//        {
//            try
//            {
//                var response = await _http.GetFromJsonAsync<MonthViewDto>(
//                    $"api/Attendance/month-view?year={year}&month={month}");
//                return response;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error fetching month view data: {ex.Message}");
//                return null;
//            }
//        }

//        // ✅ Call API for a single day's attendance
//        public async Task<List<DailyAttendanceDto>> GetDayViewAsync(DateTime date)
//        {
//            try
//            {
//                var response = await _http.GetFromJsonAsync<List<DailyAttendanceDto>>(
//                    $"api/Attendance/day-view?date={date:yyyy-MM-dd}");

//                return response ?? new List<DailyAttendanceDto>();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Error fetching day view data: {ex.Message}");
//                return new List<DailyAttendanceDto>();
//            }
//        }

//        // Get year view data
//        public async Task<YearViewDto?> GetYearViewAsync(int year)
//        {
//            try
//            {
//                return await _http.GetFromJsonAsync<YearViewDto>($"api/Attendance/year-view/{year}");
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Error fetching year view: {ex.Message}");
//                return null;
//            }
//        }

//        // Get all employees (for Employees.razor)
//        public async Task<List<EmployeeDto>> GetAllEmployeesAsync()
//        {
//            try
//            {
//                var response = await _http.GetFromJsonAsync<List<EmployeeDto>>("api/Attendance/employees");
//                return response ?? new List<EmployeeDto>();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error fetching employees: {ex.Message}");
//                return new List<EmployeeDto>();
//            }
//        }

//        // Add new employee
//        public async Task<string> AddEmployeeAsync(EmployeeDto employee)
//        {
//            var response = await _http.PostAsJsonAsync("api/Attendance/add-employee", employee);

//            if (response.IsSuccessStatusCode)
//            {
//                return "success";
//            }

//            // اقرأ رسالة الخطأ من السيرفر
//            var error = await response.Content.ReadAsStringAsync();
//            throw new Exception(error);
//        }
//        // Delete employee
//        public async Task DeleteEmployeeAsync(string Code)
//        {
//            await _http.DeleteAsync($"api/Attendance/delete-employee/{Code}");
//        }

//        // Update employee
//        public async Task<string> UpdateEmployeeAsync(UpdataEmployeecommand command)
//        {
//            try
//            {
//                var response = await _http.PutAsJsonAsync("api/Attendance/update-employee", command);

//                if (response.IsSuccessStatusCode)
//                    return "success";

//                // قراءة رسالة الخطأ من السيرفر
//                var error = await response.Content.ReadAsStringAsync();
//                return $"Error: {error}";
//            }
//            catch (Exception ex)
//            {
//                // لو حصل أي خطأ في الاتصال
//                return $"Exception: {ex.Message}";
//            }
//        }

//        // Legacy method - kept for backward compatibility
//        public async Task<List<AttendanceDayStatus>> GetAttendanceByDateAsync(DateTime date)
//        {
//            try
//            {
//                var dayData = await GetDayViewAsync(date);
//                return dayData.Select(d => new AttendanceDayStatus(
//                    d.Code,
//                    d.Date,
//                    d.ActualStatus,
//                    d.PlannedStatus, // Assuming PlannedStatus is part of DailyAttendanceDto
//                    d.ApprovalStatus, // Assuming ApprovalStatus is part of DailyAttendanceDto
//                    d.Note
//                )).ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error fetching attendance data: {ex.Message}");
//                return new List<AttendanceDayStatus>();
//            }
//        }

//        // إضافة سجل حضور جديد still not implemented
//        public async Task AddAttendanceAsync(AttendanceRecord record)
//        {
//            // This would typically call an API endpoint to add a new attendance record
//            // For now, we'll just add it to the local list
//            _records.Add(record);
//            await Task.CompletedTask;
//        }

//        public async Task<bool> UpdateEmployeeAttendanceStatusAsync(EmployeeAttendanceRequest employeeAttendance)
//        {
//            try
//            {
//                var response = await _http.PutAsJsonAsync("api/Attendance/update-attendance-record", employeeAttendance);

//                if (response.IsSuccessStatusCode)
//                {
//                    return true;
//                }

//                var errorContent = await response.Content.ReadAsStringAsync();
//                Console.WriteLine($"Error updating attendance: {response.StatusCode} - {errorContent}");
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Exception updating attendance: {ex.Message}");
//                return false;
//            }
//        }
//        public async Task<bool> UpdateEmployeeAttendanceRecordAsync(AttendanceRecord EmployeeAttedanceRecord)
//        {
//            try
//            {
//                var response = await _http.PutAsJsonAsync("api/Attendance/update-attendance-record", EmployeeAttedanceRecord);
//                if (response.IsSuccessStatusCode)
//                {
//                    return true;
//                }
//                var errorContent = await response.Content.ReadAsStringAsync();
//                Console.WriteLine($"Error updating attendance record: {response.StatusCode} - {errorContent}");
//                return false;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Exception updating attendance record: {ex.Message}");
//                return false;
//            }

//        }

//        // جدولة حضور موظف لأيام محددة (Plan Attendance)
//        public async Task<bool> PlanAttendanceAsync(string employeeCode, List<DateTime> days, AttendanceStatus plannedStatus)
//        {
//            var dto = new PlanAttendanceDto
//            {
//                Code = employeeCode,
//                Dates = days,
//                PlannedStatus = plannedStatus
//            };
//            var response = await _http.PostAsJsonAsync("api/Attendance/plan-attendance", dto);
//            return response.IsSuccessStatusCode;
//        }
//        // Get single employee by Id
//        public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id)
//        {
//            try
//            {
//                var response = await _http.GetFromJsonAsync<EmployeeDto>($"api/Attendance/employee/{id}");
//                return response;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Error fetching employee {id}: {ex.Message}");
//                return null;
//            }
//        }


//        // Get single employee by Code
//        public async Task<EmployeeDto?> GetEmployeeByCodeAsync(string code)
//        {
//            try
//            {
//                var response = await _http.GetFromJsonAsync<EmployeeeByCodeOutPut>($"api/Attendance/employee-by-code/{code}");
//                if (response?.Data == null)
//                    return null;

//                // تحويل EmployeeByCodeDto → EmployeeDto
//                var employee = new EmployeeDto
//                {
//                    Code = response.Data.Code,
//                    Name = response.Data.Name,
//                    Department = response.Data.Department,
//                    Position = response.Data.Position,
//                    Email = response.Data.Email,
//                    Phone = response.Data.Phone,
//                    Salary = response.Data.Salary,
//                    ProfileImagePath = response.Data.ProfileImagePath
//                };

//                return employee;
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"❌ Error fetching employee by code {code}: {ex.Message}");
//                return null;
//            }
//        }




//        public async Task<string?> UploadProfileImageAsync(string employeeCode, IBrowserFile file)
//        {
//            if (file == null) return null;

//            var content = new MultipartFormDataContent();
//            // تحويل IBrowserFile لـ StreamContent
//            var stream = file.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // 5MB
//            content.Add(new StreamContent(stream), "file", file.Name);

//            // POST للصورة
//            var response = await _http.PostAsync($"api/Attendance/upload-profile-image/{employeeCode}", content);

//            if (!response.IsSuccessStatusCode)
//            {
//                var error = await response.Content.ReadAsStringAsync();
//                Console.WriteLine($"❌ Upload failed: {error}");
//                return null;
//            }

//            var result = await response.Content.ReadFromJsonAsync<ProfileImageResult>();
//            return result?.ImageUrl;
//        }




//    }
//}

