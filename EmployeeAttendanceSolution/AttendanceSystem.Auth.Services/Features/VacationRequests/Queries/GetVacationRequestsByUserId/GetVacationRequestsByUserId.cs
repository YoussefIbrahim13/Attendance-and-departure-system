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

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Queries.GetVacationRequestsByUserId
{
    public record GetVacationRequestsByUserIdQuery(string UserId) : IRequest<OperationResult<List<VacationRequest>>>;

    public class GetVacationRequestsByUserIdHandler : IRequestHandler<GetVacationRequestsByUserIdQuery, OperationResult<List<VacationRequest>>>
    {
        private readonly ApplicationDbContext _dbContext;
        public GetVacationRequestsByUserIdHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public async Task<OperationResult<List<VacationRequest>>> Handle(GetVacationRequestsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var list = await _dbContext.VacationRequests
                                       .Where(v => v.UserId == request.UserId)
                                       .OrderByDescending(v => v.CreatedAt)
                                       .ToListAsync(cancellationToken);

            return new OperationResult<List<VacationRequest>> { Success = true, Data = list, Message = "Vacation requests for user retrieved successfully" };
        }
    }

}
