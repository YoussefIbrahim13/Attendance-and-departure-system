using Applications.UpdateAttendanceRecord.DTOS;
using AutoMapper;
using Domain.Entities;

namespace Applications.UpdateAttendanceRecord.profile;

public class UpdateAttendanceRecordProfile_: Profile
{
    public UpdateAttendanceRecordProfile_()
    {
        CreateMap<UpdateAttendanceRecordDto, AttendanceRecord>();
    }
}