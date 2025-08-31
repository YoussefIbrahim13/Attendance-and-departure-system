using EmployeesModels.Shared.Data;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Http;

namespace AttendanceSystem.ImportFile.API.Services.AttendanceServices
{
    public interface IAttendanceService
    {
        Task<List<AttendanceRecord>> UploadCSVFileAsync(IFormFile file);
        int GetWorkingDaysInMonth(int year, int month);
        string EditPendingAttendance(List<AttendanceRecord> pendingAttendance, EditAttendanceDto dto);
        Task<string> SavePendingAttendance(List<AttendanceRecord> pendingAttendance, ApplicationDbContext db);
        Task<bool> PlanAttendanceAsync(PlanAttendanceDto dto, ApplicationDbContext db);
        Task<MonthViewDto>GetMonthViewAsync(int year, int month, ApplicationDbContext db);
        Task<List<DailyAttendanceDto>> GetDayViewAsync(DateTime date, ApplicationDbContext db);
        Task<YearViewDto> GetYearViewAsync(int year, ApplicationDbContext db);
        Task<List<Employee>> GetEmployeesAsync(ApplicationDbContext db);
        Task<(bool Success, string Message)> DeleteEmployeeAsync(string id, ApplicationDbContext db);
        Task<(bool Success, string Message)> AddEmployeeAsync(Employee employeeDto, ApplicationDbContext db);
        Task<(bool Success, string Message)> UpdateEmployeeAsync(Employee employeeDto, ApplicationDbContext db);
        Task<bool> UpdateAttendanceRecordAsync(AttendanceRecord record, ApplicationDbContext db);


    }
}