using Applications.CSVFile.DTOS.AttendanceRecord;
using Domain.Entities;

namespace Applications.CSVFile.Profile;
using AutoMapper;


public class AttendanceRecordProfile : Profile
{
    public AttendanceRecordProfile()
    {
        CreateMap<EditAttendanceRecordDto, AttendanceRecord>();
        CreateMap<SaveAttendanceRecordDto, AttendanceRecord>();

    }
}