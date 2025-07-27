using EmployeesModels.Shared;
using Microsoft.AspNetCore.Http;

namespace AttendanceSystem.ImportFile.API.Services.AttendanceServices
{
    public interface IAttendanceService
    {
        Task<List<AttendanceRecord>> UploadCSVFileAsync(IFormFile file);
        int GetWorkingDaysInMonth(int year, int month);
    }
}