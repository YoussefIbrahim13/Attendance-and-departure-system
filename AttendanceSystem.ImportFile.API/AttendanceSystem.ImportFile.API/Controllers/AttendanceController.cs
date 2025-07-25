using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
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
                            CheckOut = checkOut
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
            if (string.IsNullOrWhiteSpace(dto.EmployeeId) || string.IsNullOrWhiteSpace(dto.Date))
                return BadRequest("EmployeeId and Date are required.");

            if (!DateTime.TryParse(dto.Date, out var date))
                return BadRequest("Invalid date format.");

            var record = _pendingAttendance.FirstOrDefault(x => x.EmployeeId == dto.EmployeeId && x.Date == date);
            if (record == null)
                return NotFound("Attendance record not found in pending data.");

            record.CheckIn = dto.CheckIn;
            record.CheckOut = dto.CheckOut;
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
    }

    // DTO للتعديل
    public class EditAttendanceDto
    {
        public string EmployeeId { get; set; }
        public string Date { get; set; } // yyyy-MM-dd
        public string CheckIn { get; set; }
        public string CheckOut { get; set; }
        public string? Note { get; set; }
    }
}