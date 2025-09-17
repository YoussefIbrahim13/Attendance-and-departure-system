using EmployeesModels.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;
using Infrastructure_.DBContext;

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Queries.GetAllVacationRequests
{
    public record GetAllVacationRequestsQuery() : IRequest<OperationResult<List<VacationRequest>>>;

    public class GetAllVacationRequestsHandler : IRequestHandler<GetAllVacationRequestsQuery, OperationResult<List<VacationRequest>>>
    {
        private readonly ApplicationDbContext _dbContext;

        public GetAllVacationRequestsHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public async Task<OperationResult<List<VacationRequest>>> Handle(GetAllVacationRequestsQuery request, CancellationToken cancellationToken)
        {
            var list = await _dbContext.VacationRequests
                                       .OrderByDescending(v => v.CreatedAt)
                                       .ToListAsync(cancellationToken);

            return new OperationResult<List<VacationRequest>> { Success = true, Data = list, Message = "Vacation requests retrieved successfully" };
        }
    }

}
