using Domain.Comman;
using Infrastructure.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Employees.Queries.GetAllEmployees
{
    public class GetAllEmployeesQuery : IRequest<List<EmployeeResult>>
    {
    }
    public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, List<EmployeeResult>>
    {
        private readonly ApplicationDbContext _db;

        public GetAllEmployeesQueryHandler(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<EmployeeResult>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
        {
            return await _db.Employees
                .Select(e => new EmployeeResult
                {
                    Id = e.Id,
                    Code = e.Code,
                    Name = e.Name,
                    Department = e.Department,
                    Position = e.Position
                })
                .ToListAsync(cancellationToken);
        }
    }
}
