using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using EmployeesModels.Shared;
using EmployeesModels.Shared.Data;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;

namespace AttendanceSystem.ImportFile.API.Services.AttendanceServices
{
    public class AttendanceService : IAttendanceService
    {
        // حفظ البيانات المؤقتة في قاعدة البيانات
        public async Task<string> SavePendingAttendance(List<AttendanceRecord> pendingAttendance, AttendanceDbContext db)
        {
            if (db == null)
                return "Invalid DbContext.";

            if (pendingAttendance.Count == 0)
                return "No pending attendance data to save.";

            var employeeIds = pendingAttendance.Select(x => x.EmployeeId).ToList();
            var dates = pendingAttendance.Select(x => x.Date).ToList();

            var existingRecords = await db.AttendanceRecords
                .Where(x => employeeIds.Contains(x.EmployeeId) && dates.Contains(x.Date))
                .ToListAsync();

            foreach (var rec in pendingAttendance)
            {
                var existing = existingRecords
                    .FirstOrDefault(x => x.EmployeeId == rec.EmployeeId && x.Date == rec.Date);

                if (existing != null)
                {
                    existing.CheckIn = rec.CheckIn;
                    existing.CheckOut = rec.CheckOut;
                    existing.ActualStatus = rec.ActualStatus;
                    existing.PlannedStatus = rec.PlannedStatus;
                    existing.ApprovalStatus = rec.ApprovalStatus;
                    existing.Note = rec.Note;
                }
                else
                {
                    db.AttendanceRecords.Add(rec);
                }
            }

            await db.SaveChangesAsync();
            pendingAttendance.Clear();
            return "Attendance data saved successfully.";
        }

        // تعديل سجل في قائمة الحضور المؤقتة
        public string EditPendingAttendance(List<AttendanceRecord> pendingAttendance, EditAttendanceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EmployeeId))
                return "EmployeeId is required.";

            var record = pendingAttendance
                .FirstOrDefault(x => x.EmployeeId == dto.EmployeeId && x.Date.Date == dto.Date.Date);

            if (record == null)
                return "Attendance record not found in pending data.";

                 record.CheckIn = dto.CheckIn;
                 record.CheckOut = dto.CheckOut;
                 record.ActualStatus = dto.ActualStatus;
                 record.Note = dto.Note;
                 record.ApprovalStatus = dto.ApprovalStatus; 


            return "Pending attendance record updated successfully.";
        }
        public async Task<List<AttendanceRecord>> UploadCSVFileAsync(IFormFile file)
        {
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
                        var checkInStr = parts[i].Trim().Replace("�", string.Empty);
                        var checkOutStr = parts[i + 1].Trim().Replace("�", string.Empty);
                        TimeSpan checkIn = TimeSpan.Zero;
                        TimeSpan checkOut = TimeSpan.Zero;
                        TimeSpan.TryParse(checkInStr, out checkIn);
                        TimeSpan.TryParse(checkOutStr, out checkOut);
                        var empId = employeeIds[empIndex];
                        attendanceRecords.Add(new AttendanceRecord
                        {
                            EmployeeId = empId,
                            Date = date,
                            CheckIn = checkIn,
                            CheckOut = checkOut,
                            ActualStatus = DetermineAttendanceStatus(checkInStr, checkOutStr),
                            ApprovalStatus = ApprovalStatus.Pending
                        });
                    }
                }
            }

            return attendanceRecords;
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
        public int GetWorkingDaysInMonth(int year, int month)
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

        public async Task<bool> PlanAttendanceAsync(PlanAttendanceDto dto, AttendanceDbContext db)
        {
            if (dto == null || db == null || string.IsNullOrWhiteSpace(dto.EmployeeId) || dto.Dates == null || dto.Dates.Count == 0)
                return false;

            foreach (var date in dto.Dates)
            {
                var record = await db.AttendanceRecords.FirstOrDefaultAsync(x => x.EmployeeId == dto.EmployeeId && x.Date == date);
                if (record != null)
                {
                       // إذا لم يتم تعديل ActualStatus يدويًا (أي يساوي PlannedStatus القديم)، حدثه مع PlannedStatus الجديد
                    if (record.ActualStatus == record.PlannedStatus)
                    {
                            record.ActualStatus = dto.PlannedStatus;
                    }
                    record.PlannedStatus = dto.PlannedStatus;
                }
                else
                {
                    db.AttendanceRecords.Add(new AttendanceRecord
                    {
                        EmployeeId = dto.EmployeeId,
                        Date = date,
                        PlannedStatus = dto.PlannedStatus,
                        ActualStatus = dto.PlannedStatus, // تعيين الحالة الفعلية = المخطط لها عند الإضافة
                        ApprovalStatus = ApprovalStatus.Pending,
                        CheckIn = TimeSpan.Zero,
                        CheckOut = TimeSpan.Zero
                    });
                }
            }
            await db.SaveChangesAsync();
            return true;
        }
    }
}
