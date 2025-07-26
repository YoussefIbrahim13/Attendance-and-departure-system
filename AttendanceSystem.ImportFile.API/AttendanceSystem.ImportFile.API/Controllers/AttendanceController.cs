using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using AttendanceSystem.ImportFile.API.Shared;

using System.Text;

namespace AttendanceSystem.ImportFile.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AttendanceController : ControllerBase
    {

        // تخزين مؤقت للبيانات المرفوعة (في الذاكرة)
        private static List<AttendanceRecord> _pendingAttendance = new();


        [HttpPost("upload-csv")]
        public async Task<IActionResult> UploadCsv(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            var attendanceRecords = new List<AttendanceRecord>();
            List<string> employeeIds = new();

            using (var stream = file.OpenReadStream())
            // استخدم UTF8 مع BOM لتفادي مشاكل الترميز
            using (var reader = new StreamReader(stream, new UTF8Encoding(true)))
            {
                await reader.ReadLineAsync();
                var empLine = await reader.ReadLineAsync();
                var empParts = empLine?.Split(';') ?? Array.Empty<string>();
                await reader.ReadLineAsync();

                for (int i = 1; i < empParts.Length; i += 2)
                {
                    var empId = empParts[i].Trim();
                    if (!string.IsNullOrEmpty(empId) && int.TryParse(empId, out _))
                        employeeIds.Add(empId);
                }

                string? line;
                while ((line = await reader.ReadLineAsync()) != null)
                {
                    var parts = line.Split(';');
                    if (parts.Length < 2) continue;
                    var dateStr = parts[0].Trim();
                    if (string.IsNullOrEmpty(dateStr) || dateStr.ToLower().Contains("total") || dateStr.ToLower().Contains("grand"))
                        continue;

                    if (!DateTime.TryParseExact(dateStr, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                        continue;

                    int empIndex = 0;
                    for (int i = 1; i < parts.Length - 1 && empIndex < employeeIds.Count; i += 2, empIndex++)
                    {
                        // نظف أي رموز غريبة (مثل �) من القيم
                        var checkIn = parts[i].Trim().Replace("�", string.Empty);
                        var checkOut = parts[i + 1].Trim().Replace("�", string.Empty);
                        // Allow adding records even if both CheckIn and CheckOut are empty, to support HR excuses in Note
                        var empId = employeeIds[empIndex];
                        attendanceRecords.Add(new AttendanceRecord
                        {
                            EmployeeId = empId,
                            Date = date,
                            CheckIn = checkIn,
                            CheckOut = checkOut,
                            Status = DetermineAttendanceStatus(checkIn, checkOut)

                        });
                    }
                }
            }

            _pendingAttendance = attendanceRecords;
            return Ok(_pendingAttendance);
        }

        // تعديل سجل في البيانات المؤقتة فقط
        [HttpPut("edit-pending")]
        public IActionResult EditPendingAttendance([FromBody] EditAttendanceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EmployeeId))
                return BadRequest("EmployeeId is required.");

            // ✅ dto.Date is already DateTime, so no parsing needed
            var record = _pendingAttendance
                .FirstOrDefault(x => x.EmployeeId == dto.EmployeeId && x.Date.Date == dto.Date.Date);

            if (record == null)
                return NotFound("Attendance record not found in pending data.");

            // ✅ Update fields
            record.CheckIn = dto.CheckIn;
            record.CheckOut = dto.CheckOut;
            record.Status = dto.Status;
            record.Note = dto.Note;

            return Ok("Pending attendance record updated successfully.");
        }


        // حفظ البيانات المؤقتة في الداتا بيز بعد موافقة HR
        [HttpPost("save")]
        public async Task<IActionResult> SaveAttendance([FromServices] AttendanceDbContext db)
        {
            if (_pendingAttendance.Count == 0)
                return BadRequest("No pending attendance data to save.");

            foreach (var rec in _pendingAttendance)
            {
                var existing = db.AttendanceRecords.FirstOrDefault(x => x.EmployeeId == rec.EmployeeId && x.Date == rec.Date);
                if (existing != null)
                {
                    existing.CheckIn = rec.CheckIn;
                    existing.CheckOut = rec.CheckOut;
                    existing.Status = rec.Status;
                    existing.Note = rec.Note; // Ensure Note is updated as well
                }
                else
                {
                    db.AttendanceRecords.Add(rec);
                }
            }
            await db.SaveChangesAsync();
            _pendingAttendance.Clear();
            return Ok("Attendance data saved successfully.");
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
                Console.WriteLine($"DB record → EmpID={r.EmployeeId}, Date={r.Date:yyyy-MM-dd}, Status={r.Status}");
            }

            // 4️⃣ Get all employees
            var employees = await db.Employees.ToListAsync();
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
                        Status = attendance?.Status ?? AttendanceStatus.Absent,
                        Note = attendance?.Note
                    };
                }).ToList();

                // ✅ Count present/absent employees
                var presentCount = employeeStatuses.Count(es => es.Status == AttendanceStatus.Present);
                var absentCount = employeeStatuses.Count(es => es.Status == AttendanceStatus.Absent);

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
                    Date = day,
                    CheckIn = attendance?.CheckIn ?? string.Empty,
                    CheckOut = attendance?.CheckOut ?? string.Empty,
                    Status = attendance?.Status ?? AttendanceStatus.Absent,
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

                // ✅ Calculate working days (weekends excluded if needed)
                int workingDays = GetWorkingDaysInMonth(year, month);

                // ✅ Calculate metrics
                int totalPossibleAttendance = totalEmployees * workingDays;
                int actualAttendance = attendanceData.Count(ar => ar.Status == AttendanceStatus.Present);

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

        // Add new employee
        [HttpPost("add-employee")]
        public async Task<IActionResult> AddEmployee([FromServices] AttendanceDbContext db, [FromBody] AttendanceSystem.ImportFile.API.Shared.Employee employeeDto)
        {
            if (employeeDto == null || string.IsNullOrWhiteSpace(employeeDto.Id) || string.IsNullOrWhiteSpace(employeeDto.Name))
                return BadRequest("Employee data is required.");

            var exists = await db.Employees.AnyAsync(e => e.Id == employeeDto.Id);
            if (exists)
                return BadRequest("Employee with this ID already exists.");

            db.Employees.Add(employeeDto);
            await db.SaveChangesAsync();
            return Ok("Employee added successfully.");
        }

        // Update employee
        [HttpPut("update-employee")]
        public async Task<IActionResult> UpdateEmployee([FromServices] AttendanceDbContext db, [FromBody] AttendanceSystem.ImportFile.API.Shared.Employee employeeDto)
        {
            if (employeeDto == null || string.IsNullOrWhiteSpace(employeeDto.Id))
                return BadRequest("Employee data is required.");

            var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == employeeDto.Id);
            if (employee == null)
                return NotFound("Employee not found.");

            employee.Name = employeeDto.Name;
            employee.Department = employeeDto.Department;
            employee.Position = employeeDto.Position;

            await db.SaveChangesAsync();
            return Ok("Employee updated successfully.");
        }

        // Helper method to determine attendance status
        private AttendanceStatus DetermineAttendanceStatus(string checkIn, string checkOut)
        {
            if (string.IsNullOrEmpty(checkIn) && string.IsNullOrEmpty(checkOut))
                return AttendanceStatus.Absent;

            if (!string.IsNullOrEmpty(checkIn))
            {
              
                return AttendanceStatus.Present;
            }

            return AttendanceStatus.Present;
        }

        // Helper method to get working days in a month (excluding weekends)
        private int GetWorkingDaysInMonth(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);
            int workingDays = 0;

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday)
                    workingDays++;
            }

            return workingDays;
        }
    }

    // DTO للتعديل
    public class EditAttendanceDto
    {
        public string EmployeeId { get; set; }
        public DateTime Date { get; set; } // yyyy-MM-dd
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }
}