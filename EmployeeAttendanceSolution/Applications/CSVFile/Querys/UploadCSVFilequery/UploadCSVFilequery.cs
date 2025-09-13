using Applications.CSVFile.DTOS.AttendanceRecord;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Applications.CSVFile.Querys.UploadCSVFilequery;

public class UploadCSVFilequery: IRequest<List<AttendanceRecordDto>>
{
    public IFormFile File { get; }

    public UploadCSVFilequery(IFormFile file)
    {
        File = file;
    }

}