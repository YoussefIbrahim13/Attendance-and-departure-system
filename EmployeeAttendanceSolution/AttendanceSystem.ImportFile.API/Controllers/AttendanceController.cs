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
        public async Task<IActionResult> SaveAttendance([FromServices] AttendanceDbContext db)
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
        public async Task<IActionResult> GetMonthView([FromServices] AttendanceDbContext db, int year, int month)
        {
            // 1️⃣ Determine the date range for the month
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            Console.WriteLine($"📅 Fetching records for {month}/{year} → from {startDate} to {endDate}");

            // 2️⃣ Get attendance records for that month from DB
            var attendanceData = await db.AttendanceRecords
                .Where(ar => ar.Date >= startDate && ar.Date <= endDate)
                .ToListAsync();

            Console.WriteLine($"✅ Retrieved {attendanceData.Count} attendance records for {month}/{year}");

            // 3️⃣ Debugging: print some sample records
            foreach (var r in attendanceData.Take(5)) // show only first 5
            {
                Console.WriteLine($"DB record → EmpID={r.EmployeeId}, Date={r.Date:yyyy-MM-dd}, Status={r.ActualStatus}");
            }

            // 4️⃣ Get all employees
            var employees = await db.Employees.ToListAsync();
        // SRP: تجهيز بيانات الشهر للواجهة فقط، وتعتمد على الخدمات عبر DI
            Console.WriteLine($"✅ Total Employees: {employees.Count}");

            // 5️⃣ Prepare DTO for UI
            var monthViewDto = new MonthViewDto
            {
                Year = year,
                Month = month,
                Days = new List<CalendarDayDto>()
            };

            // 6️⃣ Loop through each day of the month
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                // ✅ Filter attendance for the current date
                var dayAttendance = attendanceData
                    .Where(ar => ar.Date.Date == date.Date)
                    .ToList();

                // ✅ Create a list of employee statuses for this day
                var employeeStatuses = employees.Select(emp =>
                {
                    var attendance = dayAttendance.FirstOrDefault(ar => ar.EmployeeId == emp.Id);
                    return new EmployeeDayStatus
                    {
                        EmployeeId = emp.Id,
                        EmployeeName = emp.Name,
                        ActualStatus = attendance?.ActualStatus ?? AttendanceStatus.Absent,
                        Note = attendance?.Note
                    };
                }).ToList();

                // ✅ Count present/absent employees
                var presentCount = employeeStatuses.Count(es => es.ActualStatus == AttendanceStatus.Present);
                var absentCount = employeeStatuses.Count(es => es.ActualStatus == AttendanceStatus.Absent);

                Console.WriteLine($"{date:yyyy-MM-dd} → Present: {presentCount}, Absent: {absentCount}");

                // ✅ Add data for this day to the DTO
                monthViewDto.Days.Add(new CalendarDayDto
                {
                    Date = date,
                    TopEmployees = employeeStatuses.Take(4).ToList(), // Only top 4 for display in calendar
                    TotalEmployees = employees.Count,
                    PresentCount = presentCount,
                    AbsentCount = absentCount
                });
            }

            // ✅ Return to UI
            return Ok(monthViewDto);
        }



        // Get day view data
        [HttpGet("day-view")]
        public async Task<IActionResult> GetDayView([FromServices] AttendanceDbContext db, DateTime date)
        {
            // ✅ Normalize date (ignore time part for safety)
            var day = date.Date;

            // ✅ Load all employees and attendance records for the given day
            var employees = await db.Employees.ToListAsync();
            var attendanceData = await db.AttendanceRecords
                .Where(ar => ar.Date.Date == day)
                .ToListAsync();

            // ✅ Map to DTO
            var dailyAttendance = employees.Select(emp =>
            {
                var attendance = attendanceData.FirstOrDefault(ar => ar.EmployeeId == emp.Id);

                return new DailyAttendanceDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.Name,
                    Department = emp.Department,
                   // SRP: تجهيز بيانات يوم واحد للواجهة فقط
                    Date = day,
                    CheckIn = attendance?.CheckIn ?? TimeSpan.Zero,
                    CheckOut = attendance?.CheckOut ?? TimeSpan.Zero,
                    ActualStatus = attendance?.ActualStatus ?? AttendanceStatus.Absent,
                    PlannedStatus = attendance?.PlannedStatus ?? AttendanceStatus.Absent,
                    ApprovalStatus = attendance?.ApprovalStatus ?? ApprovalStatus.Pending,
                    Note = attendance?.Note ?? string.Empty
                };
            }).ToList();

            return Ok(dailyAttendance);
        }

        // Get year view data
        [HttpGet("year-view/{year}")]
        public async Task<IActionResult> GetYearView([FromServices] AttendanceDbContext db, int year)
        {
            var yearViewDto = new YearViewDto
            {
                Year = year,
                Months = new List<MonthSummaryDto>()
            };

            // ✅ Load all employees ONCE
            var totalEmployees = await db.Employees.CountAsync();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                // ✅ Fetch attendance for the month
                var attendanceData = await db.AttendanceRecords
                    .Where(ar => ar.Date >= startDate && ar.Date <= endDate)
                    .ToListAsync();

        // SRP: تجهيز ملخص السنة فقط
                // ✅ Calculate working days (weekends excluded if needed)
                int workingDays = attendanceService.GetWorkingDaysInMonth(year, month);

                // ✅ Calculate metrics
                int totalPossibleAttendance = totalEmployees * workingDays;
                int actualAttendance = attendanceData.Count(ar => ar.ActualStatus == AttendanceStatus.Present);

                double averageAttendance = totalPossibleAttendance > 0
                    ? (double)actualAttendance / totalPossibleAttendance * 100
                    : 0;

                // ✅ Add month summary
                yearViewDto.Months.Add(new MonthSummaryDto
                {
                    Month = month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalWorkingDays = workingDays,
                    AverageAttendance = Math.Round(averageAttendance, 2)
                });
            }

            return Ok(yearViewDto);
        }

        // Get all employees
        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees([FromServices] AttendanceDbContext db)
        {
            var employees = await db.Employees.ToListAsync();
            return Ok(employees);
        }

        // Delete employee
        [HttpDelete("delete-employee/{id}")]
        public async Task<IActionResult> DeleteEmployee([FromServices] AttendanceDbContext db, string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest("Employee ID is required.");

            var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
                return NotFound("Employee not found.");

            db.Employees.Remove(employee);
            await db.SaveChangesAsync();
            return Ok("Employee deleted successfully.");
        }
        // SRP: جلب كل الموظفين فقط

        // Add new employee
        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployee([FromServices] AttendanceDbContext db, [FromBody] Employee employeeDto)
        {
            if (employeeDto == null || string.IsNullOrWhiteSpace(employeeDto.Id) || string.IsNullOrWhiteSpace(employeeDto.Name))
                return BadRequest("Employee data is required.");

        // SRP: حذف موظف فقط
            var exists = await db.Employees.AnyAsync(e => e.Id == employeeDto.Id);
            if (exists)
                return BadRequest("Employee with this ID already exists.");

            db.Employees.Add(employeeDto);
            await db.SaveChangesAsync();
            return Ok("Employee added successfully.");
        }

        // Update employee
        [HttpPut("update-employee")]
        public async Task<IActionResult> UpdateEmployee([FromServices] AttendanceDbContext db, [FromBody] Employee employeeDto)
        {
            if (employeeDto == null || string.IsNullOrWhiteSpace(employeeDto.Id))
                return BadRequest("Employee data is required.");

        // SRP: إضافة موظف جديد فقط
            var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == employeeDto.Id);
            if (employee == null)
                return NotFound("Employee not found.");

            employee.Name = employeeDto.Name;
            employee.Department = employeeDto.Department;
            employee.Position = employeeDto.Position;

            await db.SaveChangesAsync();
            return Ok("Employee updated successfully.");
        }

        // إضافة خطة حضور لموظف لأيام محددة (API Endpoint)
        [HttpPost("plan-attendance")]
        public async Task<IActionResult> PlanAttendance([FromServices] AttendanceDbContext db, [FromBody] PlanAttendanceDto dto)
        {
            var ok = await attendanceService.PlanAttendanceAsync(dto, db);
            if (ok)
                return Ok("Attendance plan saved successfully.");
            return BadRequest("Failed to save attendance plan.");
        }
        // تحديث سجل حضور موظف ليوم معين
        [HttpPut("update-attendance-record")]
        public async Task<IActionResult> UpdateAttendanceRecord([FromServices] AttendanceDbContext db, [FromBody] AttendanceRecord record)
        {
            if (record == null || string.IsNullOrWhiteSpace(record.EmployeeId))
                return BadRequest("Attendance record data is required.");

            var existing = await db.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeId == record.EmployeeId && a.Date == record.Date);
            if (existing == null)
            {
                // إذا لم يوجد سجل، أنشئ واحد جديد
                db.AttendanceRecords.Add(new AttendanceRecord
                {
                    EmployeeId = record.EmployeeId,
                    Date = record.Date,
                    ActualStatus = record.ActualStatus,
                    PlannedStatus = record.PlannedStatus,
                    ApprovalStatus = record.ApprovalStatus,
                    CheckIn = record.CheckIn,
                    CheckOut = record.CheckOut,
                    Note = record.Note
                });
            }
            else
            {
                existing.ActualStatus = record.ActualStatus;
                existing.PlannedStatus = record.PlannedStatus;
                existing.ApprovalStatus = record.ApprovalStatus;
                existing.CheckIn = record.CheckIn;
                existing.CheckOut = record.CheckOut;
                existing.Note = record.Note;
            }

            await db.SaveChangesAsync();
            return Ok(true);
        }
    }
}