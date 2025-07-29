using EmployeesModels.Shared;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;

namespace AttendanceSystem.ImportFile.API.Services.AttendanceServices
{
    public class AttendanceService : IAttendanceService
    {
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

                        // Try parse or leave default (TimeSpan.Zero)
                        TimeSpan.TryParse(checkInStr, out checkIn);
                        TimeSpan.TryParse(checkOutStr, out checkOut);
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

            return attendanceRecords;
        }

        // Helper method to determine attendance status
        private AttendanceStatus DetermineAttendanceStatus(TimeSpan checkIn, TimeSpan checkOut)
        {
            if (checkIn == TimeSpan.Zero && checkOut == TimeSpan.Zero)
                return AttendanceStatus.Absent;

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

    }
}
