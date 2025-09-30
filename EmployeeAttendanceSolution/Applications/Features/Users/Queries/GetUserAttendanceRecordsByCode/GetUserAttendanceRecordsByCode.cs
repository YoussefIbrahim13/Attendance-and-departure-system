using Domain.Comman;
using Domain.Entities;
using Infrastructure.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Applications.Features.Users.Queries.GetUserAttendanceRecordsByCode
{
    public record  GetUserAttendanceRecordsByCode(string code ):IRequest <OperationResult<List<AttendanceRecord>>>;
    public class GetUserAttendanceRecordsByCodeHandler : IRequestHandler<GetUserAttendanceRecordsByCode, OperationResult<List<AttendanceRecord>>>
    {
        private readonly ApplicationDbContext _dbContext;
        public GetUserAttendanceRecordsByCodeHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;
        public async Task<OperationResult<List<AttendanceRecord>>> Handle(GetUserAttendanceRecordsByCode request, CancellationToken cancellationToken)
        {
            var list = _dbContext.AttendanceRecords
                                       .Where(v => v.Code == request.code)
                                       .OrderByDescending(v => v.Date)
                                       .ToList();
            return new OperationResult<List<AttendanceRecord>> { Success = true, Data = list , Message = "Attendance records for user retrieved successfully" };
        }
    }

}
