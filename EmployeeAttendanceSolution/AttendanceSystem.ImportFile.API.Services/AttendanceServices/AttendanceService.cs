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
        public async Task<string> SavePendingAttendance(List<AttendanceRecord> pendingAttendance, ApplicationDbContext db)
        {
            if (db == null)
                return "Invalid DbContext.";

            if (pendingAttendance.Count == 0)
                return "No pending attendance data to save.";

            var employeeIds = pendingAttendance.Select(x => x.Code).ToList();
            var dates = pendingAttendance.Select(x => x.Date).ToList();

            var existingRecords = await db.AttendanceRecords
                .Where(x => employeeIds.Contains(x.Code) && dates.Contains(x.Date))
                .ToListAsync();

            foreach (var rec in pendingAttendance)
            {
                var existing = existingRecords
                    .FirstOrDefault(x => x.Code == rec.Code && x.Date == rec.Date);

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
            if (string.IsNullOrWhiteSpace(dto.Code))
                return "EmployeeCode is required.";

            var record = pendingAttendance
                .FirstOrDefault(x => x.Code == dto.Code && x.Date.Date == dto.Date.Date);

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
            List<string> employeeCodes = new();

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
                    var empCode = empParts[i].Trim();
                    if (!string.IsNullOrEmpty(empCode))
                        employeeCodes.Add(empCode);
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
                    for (int i = 1; i < parts.Length - 1 && empIndex < employeeCodes.Count; i += 2, empIndex++)
                    {
                        // نظف أي رموز غريبة (مثل �) من القيم
                        var checkInStr = parts[i].Trim().Replace("�", string.Empty);
                        var checkOutStr = parts[i + 1].Trim().Replace("�", string.Empty);
                        TimeSpan checkIn = TimeSpan.Zero;
                        TimeSpan checkOut = TimeSpan.Zero;
                        TimeSpan.TryParse(checkInStr, out checkIn);
                        TimeSpan.TryParse(checkOutStr, out checkOut);
                        var empCode = employeeCodes[empIndex];
                        attendanceRecords.Add(new AttendanceRecord
                        {
                            Code = empCode,
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

        public async Task<bool> PlanAttendanceAsync(PlanAttendanceDto dto, ApplicationDbContext db)
        {
            if (dto == null || db == null || string.IsNullOrWhiteSpace(dto.Code) || dto.Dates == null || dto.Dates.Count == 0)
                return false;

            foreach (var date in dto.Dates)
            {
                var record = await db.AttendanceRecords.FirstOrDefaultAsync(x => x.Code == dto.Code && x.Date == date);
                if (record != null)
                {
                       
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
                        Code = dto.Code,
                        Date = date,
                        PlannedStatus = dto.PlannedStatus,
                        ActualStatus = dto.PlannedStatus, 
                        ApprovalStatus = ApprovalStatus.Pending,
                        CheckIn = TimeSpan.Zero,
                        CheckOut = TimeSpan.Zero
                    });
                }
            }
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<MonthViewDto> GetMonthViewAsync(int year, int month, ApplicationDbContext db)
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
                    var attendance = dayAttendance.FirstOrDefault(ar => ar.Code == emp.Code);
                    return new EmployeeDayStatus
                    {
                        Code = emp.Code,
                        EmployeeName = emp.Name,
                        ActualStatus = attendance?.ActualStatus ?? AttendanceStatus.No_status,
                        Note = attendance?.Note
                    };
                }).ToList();

                var presentCount = employeeStatuses.Count(es => es.ActualStatus == AttendanceStatus.Present);
                var absentCount = employeeStatuses.Count(es => es.ActualStatus == AttendanceStatus.Absent);

                monthViewDto.Days.Add(new CalendarDayDto
                {
                    Date = date,
                    TopEmployees = employeeStatuses.Take(4).ToList(),
                    AllEmployees = employeeStatuses.ToList(),
                    TotalEmployees = employees.Count,
                    PresentCount = presentCount,
                    AbsentCount = absentCount
                });
            }

            return monthViewDto;
        }

        public async Task<List<DailyAttendanceDto>> GetDayViewAsync(DateTime date, ApplicationDbContext db)
        {
            var day = date.Date;

            var employees = await db.Employees.ToListAsync();
            var attendanceData = await db.AttendanceRecords
                .Where(ar => ar.Date.Date == day)
                .ToListAsync();

            var dailyAttendance = employees.Select(emp =>
            {
                var attendance = attendanceData.FirstOrDefault(ar => ar.Code == emp.Code);

                return new DailyAttendanceDto
                {
                    Code = emp.Code,
                    EmployeeName = emp.Name,
                    Department = emp.Department.ToString(),
                    Date = day,
                    CheckIn = attendance?.CheckIn ?? TimeSpan.Zero,
                    CheckOut = attendance?.CheckOut ?? TimeSpan.Zero,
                    ActualStatus = attendance?.ActualStatus ?? AttendanceStatus.No_status,
                    PlannedStatus = attendance?.PlannedStatus ?? AttendanceStatus.No_status,
                    ApprovalStatus = attendance?.ApprovalStatus ?? ApprovalStatus.Pending,
                    Note = attendance?.Note ?? string.Empty
                };
            }).ToList();

            return dailyAttendance;
        }

        public async Task<YearViewDto> GetYearViewAsync(int year, ApplicationDbContext db)
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

        public async Task<List<Employee>> GetEmployeesAsync(ApplicationDbContext db)
        {
            return await db.Employees.ToListAsync();
        }
        public async Task<(bool Success, string Message)> DeleteEmployeeAsync(string code, ApplicationDbContext db)
        {
            if (string.IsNullOrWhiteSpace(code))
                return (false, "Employee Code is required.");

            var employee = await db.Employees.FirstOrDefaultAsync(e => e.Code ==code);
            if (employee == null)
                return (false, "Employee not found.");

            db.Employees.Remove(employee);
            await db.SaveChangesAsync();
            return (true, "Employee deleted successfully.");
        }
        public async Task<(bool Success, string Message)> AddEmployeeAsync(Employee employeeDto, ApplicationDbContext db)
        {
            if (employeeDto == null || string.IsNullOrWhiteSpace(employeeDto.Code) || string.IsNullOrWhiteSpace(employeeDto.Name))
                return (false, "Employee data is required.");

            var exists = await db.Employees.AnyAsync(e => e.Code == employeeDto.Code);
            if (exists)
                return (false, "Employee with this Code already exists.");

            db.Employees.Add(employeeDto);
            await db.SaveChangesAsync();

            return (true, "Employee added successfully.");
        }
        public async Task<(bool Success, string Message)> UpdateEmployeeAsync(Employee employeeDto, ApplicationDbContext db)
        {
            if (employeeDto == null || string.IsNullOrWhiteSpace(employeeDto.Code))
                return (false, "Employee data is required.");

            var employee = await db.Employees.FirstOrDefaultAsync(e => e.Code == employeeDto.Code);
            if (employee == null)
                return (false, "Employee not found.");

            employee.Name = employeeDto.Name;
            employee.Department = employeeDto.Department;
            employee.Position = employeeDto.Position;

            await db.SaveChangesAsync();
            return (true, "Employee updated successfully.");
        }
        public async Task<bool> UpdateAttendanceRecordAsync(AttendanceRecord record, ApplicationDbContext db)
        {
            if (record == null || string.IsNullOrWhiteSpace(record.Code))
                return false;

            var existing = await db.AttendanceRecords.FirstOrDefaultAsync(a => a.Code == record.Code && a.Date == record.Date);

            if (existing == null)
            {
                db.AttendanceRecords.Add(new AttendanceRecord
                {
                    Code = record.Code,
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
