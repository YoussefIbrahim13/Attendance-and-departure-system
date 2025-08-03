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
        Task<string> SavePendingAttendance(List<AttendanceRecord> pendingAttendance, AttendanceDbContext db);
        Task<bool> PlanAttendanceAsync(PlanAttendanceDto dto, AttendanceDbContext db);
        Task<MonthViewDto>GetMonthViewAsync(int year, int month, AttendanceDbContext db);
        Task<List<DailyAttendanceDto>> GetDayViewAsync(DateTime date, AttendanceDbContext db);
        Task<YearViewDto> GetYearViewAsync(int year, AttendanceDbContext db);
        Task<List<Employee>> GetEmployeesAsync(AttendanceDbContext db);
        Task<(bool Success, string Message)> DeleteEmployeeAsync(string id, AttendanceDbContext db);
        Task<(bool Success, string Message)> AddEmployeeAsync(Employee employeeDto, AttendanceDbContext db);
        Task<(bool Success, string Message)> UpdateEmployeeAsync(Employee employeeDto, AttendanceDbContext db);
        Task<bool> UpdateAttendanceRecordAsync(AttendanceRecord record, AttendanceDbContext db);


    }
}