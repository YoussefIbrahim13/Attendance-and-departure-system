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

        public async Task<MonthViewDto> GetMonthViewAsync(int year, int month, AttendanceDbContext db)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var attendanceData = await db.AttendanceRecords
                .Where(ar => ar.Date >= startDate && ar.Date <= endDate)
                .ToListAsync();

            var employees = await db.Employees.ToListAsync();

            var monthViewDto = new MonthViewDto
            {
                Year = year,
                Month = month,
                Days = new List<CalendarDayDto>()
            };

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dayAttendance = attendanceData
                    .Where(ar => ar.Date.Date == date.Date)
                    .ToList();

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

                var presentCount = employeeStatuses.Count(es => es.ActualStatus == AttendanceStatus.Present);
                var absentCount = employeeStatuses.Count(es => es.ActualStatus == AttendanceStatus.Absent);

                monthViewDto.Days.Add(new CalendarDayDto
                {
                    Date = date,
                    TopEmployees = employeeStatuses.Take(4).ToList(),
                    TotalEmployees = employees.Count,
                    PresentCount = presentCount,
                    AbsentCount = absentCount
                });
            }

            return monthViewDto;
        }

        public async Task<List<DailyAttendanceDto>> GetDayViewAsync(DateTime date, AttendanceDbContext db)
        {
            var day = date.Date;

            var employees = await db.Employees.ToListAsync();
            var attendanceData = await db.AttendanceRecords
                .Where(ar => ar.Date.Date == day)
                .ToListAsync();

            var dailyAttendance = employees.Select(emp =>
            {
                var attendance = attendanceData.FirstOrDefault(ar => ar.EmployeeId == emp.Id);

                return new DailyAttendanceDto
                {
                    EmployeeId = emp.Id,
                    EmployeeName = emp.Name,
                    Department = emp.Department,
                    Date = day,
                    CheckIn = attendance?.CheckIn ?? TimeSpan.Zero,
                    CheckOut = attendance?.CheckOut ?? TimeSpan.Zero,
                    ActualStatus = attendance?.ActualStatus ?? AttendanceStatus.Absent,
                    PlannedStatus = attendance?.PlannedStatus ?? AttendanceStatus.Absent,
                    ApprovalStatus = attendance?.ApprovalStatus ?? ApprovalStatus.Pending,
                    Note = attendance?.Note ?? string.Empty
                };
            }).ToList();

            return dailyAttendance;
        }

        public async Task<YearViewDto> GetYearViewAsync(int year, AttendanceDbContext db)
        {
            var yearViewDto = new YearViewDto
            {
                Year = year,
                Months = new List<MonthSummaryDto>()
            };

            // Load all employees ONCE
            var totalEmployees = await db.Employees.CountAsync();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                var attendanceData = await db.AttendanceRecords
                    .Where(ar => ar.Date >= startDate && ar.Date <= endDate)
                    .ToListAsync();

                int workingDays = GetWorkingDaysInMonth(year, month);

                int totalPossibleAttendance = totalEmployees * workingDays;
                int actualAttendance = attendanceData.Count(ar => ar.ActualStatus == AttendanceStatus.Present);

                double averageAttendance = totalPossibleAttendance > 0
                    ? (double)actualAttendance / totalPossibleAttendance * 100
                    : 0;

                yearViewDto.Months.Add(new MonthSummaryDto
                {
                    Month = month,
                    MonthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month),
                    TotalWorkingDays = workingDays,
                    AverageAttendance = Math.Round(averageAttendance, 2)
                });
            }

            return yearViewDto;
        }

        public async Task<List<Employee>> GetEmployeesAsync(AttendanceDbContext db)
        {
            return await db.Employees.ToListAsync();
        }
        public async Task<(bool Success, string Message)> DeleteEmployeeAsync(string id, AttendanceDbContext db)
        {
            if (string.IsNullOrWhiteSpace(id))
                return (false, "Employee ID is required.");

            var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null)
                return (false, "Employee not found.");

            db.Employees.Remove(employee);
            await db.SaveChangesAsync();
            return (true, "Employee deleted successfully.");
        }
        public async Task<(bool Success, string Message)> AddEmployeeAsync(Employee employeeDto, AttendanceDbContext db)
        {
            if (employeeDto == null || string.IsNullOrWhiteSpace(employeeDto.Id) || string.IsNullOrWhiteSpace(employeeDto.Name))
                return (false, "Employee data is required.");

            var exists = await db.Employees.AnyAsync(e => e.Id == employeeDto.Id);
            if (exists)
                return (false, "Employee with this ID already exists.");

            db.Employees.Add(employeeDto);
            await db.SaveChangesAsync();

            return (true, "Employee added successfully.");
        }
        public async Task<(bool Success, string Message)> UpdateEmployeeAsync(Employee employeeDto, AttendanceDbContext db)
        {
            if (employeeDto == null || string.IsNullOrWhiteSpace(employeeDto.Id))
                return (false, "Employee data is required.");

            var employee = await db.Employees.FirstOrDefaultAsync(e => e.Id == employeeDto.Id);
            if (employee == null)
                return (false, "Employee not found.");

            employee.Name = employeeDto.Name;
            employee.Department = employeeDto.Department;
            employee.Position = employeeDto.Position;

            await db.SaveChangesAsync();
            return (true, "Employee updated successfully.");
        }
        public async Task<bool> UpdateAttendanceRecordAsync(AttendanceRecord record, AttendanceDbContext db)
        {
            if (record == null || string.IsNullOrWhiteSpace(record.EmployeeId))
                return false;

            var existing = await db.AttendanceRecords.FirstOrDefaultAsync(a => a.EmployeeId == record.EmployeeId && a.Date == record.Date);

            if (existing == null)
            {
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
            return true;
        }



    }
}
