using Microsoft.AspNetCore.Http;

namespace Applications.CSVFile.DTOS.AttendanceRecord;

public class UploadCsvDto
{
    public IFormFile File { get; set; } = default!;
}