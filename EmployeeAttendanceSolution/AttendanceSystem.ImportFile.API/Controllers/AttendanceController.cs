using EmployeesModels.Shared.Data;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AttendanceSystem.ImportFile.API.Services.AttendanceServices;
using System.Globalization;
using System.Text;

namespace AttendanceSystem.ImportFile.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AttendanceController : ControllerBase
    {
        public AttendanceController(IAttendanceService attendanceService)
        {
            this.attendanceService = attendanceService;
        }
        // تخزين مؤقت للبيانات المرفوعة (في الذاكرة)
        private static List<AttendanceRecord> _pendingAttendance = new();
        private readonly IAttendanceService attendanceService;

        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");
            _pendingAttendance = await attendanceService.UploadCSVFileAsync(file);

            return Ok(_pendingAttendance);
        }

        // تعديل سجل في البيانات المؤقتة فقط


        [HttpPut("edit-pending")]
        public IActionResult EditPendingAttendance([FromBody] EditAttendanceDto dto)
        {
            // تمرير الطلب إلى AttendanceService عبر الواجهة
            var result = attendanceService.EditPendingAttendance(_pendingAttendance, dto);
            if (result == "EmployeeId is required.")
                return BadRequest(result);
            if (result == "Attendance record not found in pending data.")
                return NotFound(result);
            return Ok(result);
        }


        // حفظ البيانات المؤقتة في الداتا بيز بعد موافقة HR
        [HttpPost("save")]
        public async Task<IActionResult> SaveAttendance([FromServices] ApplicationDbContext db)
        {
            var result = await attendanceService.SavePendingAttendance(_pendingAttendance, db);
            if (result == "No pending attendance data to save.")
                return BadRequest(result);
            if (result == "Invalid DbContext.")
                return StatusCode(500, result);
            return Ok(result);
        }
        // Get month view data
        [HttpGet("month-view")]
        public async Task<IActionResult> GetMonthView([FromServices] ApplicationDbContext db, int year, int month)
        {
            var result = await attendanceService.GetMonthViewAsync(year, month, db);

            return Ok(result);
        }



        // Get day view data
        [HttpGet("day-view")]
        public async Task<IActionResult> GetDayView([FromServices] ApplicationDbContext db, DateTime date)
        {
               var result = await attendanceService.GetDayViewAsync(date, db);
        return Ok(result);
        }

        // Get year view data
        [HttpGet("year-view/{year}")]
        public async Task<IActionResult> GetYearView([FromServices] ApplicationDbContext db, int year)
        {
                var result = await attendanceService.GetYearViewAsync(year, db);
                return Ok(result);
            }

    // Get all employees
    [HttpGet("employees")]
    public async Task<IActionResult> GetEmployees([FromServices] ApplicationDbContext db)
    {
            var result = await attendanceService.GetEmployeesAsync(db);
            return Ok(result);
        }

    // Delete employee
    [HttpDelete("delete-employee/{id}")]
    public async Task<IActionResult> DeleteEmployee([FromServices] ApplicationDbContext db, string id)
    {
            var result = await attendanceService.DeleteEmployeeAsync(id, db);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }
    // SRP: جلب كل الموظفين فقط

    // Add new employee
    [HttpPost("add-employee")]
    public async Task<IActionResult> AddEmployee([FromServices] ApplicationDbContext db, [FromBody] Employee employeeDto)
    {
            var result = await attendanceService.AddEmployeeAsync(employeeDto, db);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

    // Update employee
    [HttpPut("update-employee")]
    public async Task<IActionResult> UpdateEmployee([FromServices] ApplicationDbContext db, [FromBody] Employee employeeDto)
    {
            var result = await attendanceService.UpdateEmployeeAsync(employeeDto, db);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

    // إضافة خطة حضور لموظف لأيام محددة (API Endpoint)
    [HttpPost("plan-attendance")]
    public async Task<IActionResult> PlanAttendance([FromServices] ApplicationDbContext db, [FromBody] PlanAttendanceDto dto)
    {
        var ok = await attendanceService.PlanAttendanceAsync(dto, db);
        if (ok)
            return Ok("Attendance plan saved successfully.");
        return BadRequest("Failed to save attendance plan.");
    }
    // تحديث سجل حضور موظف ليوم معين
    [HttpPut("update-attendance-record")]
    public async Task<IActionResult> UpdateAttendanceRecord([FromServices] ApplicationDbContext db, [FromBody] AttendanceRecord record)
    {
            var result = await attendanceService.UpdateAttendanceRecordAsync(record, db);
            if (!result)
                return BadRequest("Attendance record data is required.");

            return Ok(true);
        }
}
}