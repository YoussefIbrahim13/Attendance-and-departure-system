using EmployeesModels.Shared;
using EmployeesModels.Shared.Data;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure;

namespace AttendanceSystem.ImportFile.API.Services.AttendanceServices
{
    public interface IAttendanceService
    {
        Task<List<AttendanceRecord>> UploadCSVFileAsync(IFormFile file);//
        int GetWorkingDaysInMonth(int year, int month);//
        string EditPendingAttendance(List<AttendanceRecord> pendingAttendance, EditAttendanceDto dto);//
        Task<string> SavePendingAttendance(List<AttendanceRecord> pendingAttendance, AppDbcontext db);//
        Task<bool> PlanAttendanceAsync(PlanAttendanceDto dto, AppDbcontext db);//
        Task<MonthViewDto> GetMonthViewAsync(int year, int month, AppDbcontext db);//
        Task<List<DailyAttendanceDto>> GetDayViewAsync(DateTime date, AppDbcontext db);//
        Task<YearViewDto> GetYearViewAsync(int year, AppDbcontext db);//
        Task<List<Employee>> GetEmployeesAsync(AppDbcontext db);//
        Task<(bool Success, string Message)> DeleteEmployeeAsync(string id, AppDbcontext db);//
        Task<(bool Success, string Message)> AddEmployeeAsync(Employee employeeDto, AppDbcontext db);//
        Task<(bool Success, string Message)> UpdateEmployeeAsync(Employee employeeDto, AppDbcontext db);//
        Task<bool> UpdateAttendanceRecordAsync(AttendanceRecord record, AppDbcontext db);//  
        Task<Employee?> GetEmployeeByCodeAsync(string code, AppDbcontext db);//
        Task<(bool Success, string Message, string? ImageUrl)> UploadProfileImageAsync(string code, IFormFile file, AppDbcontext db, HttpContext httpContext);//



    }
}