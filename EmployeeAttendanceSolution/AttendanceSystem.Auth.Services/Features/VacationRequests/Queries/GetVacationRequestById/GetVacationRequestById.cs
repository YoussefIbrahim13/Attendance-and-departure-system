using EmployeesModels.Shared;
using EmployeesModels.Shared.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Queries.GetVacationRequestById
{
    public record GetVacationRequestByIdQuery(string RequestId) : IRequest<OperationResult<VacationRequest>>;

    public class GetVacationRequestByIdHandler : IRequestHandler<GetVacationRequestByIdQuery, OperationResult<VacationRequest>>
    {
        private readonly ApplicationDbContext _dbContext;
        public GetVacationRequestByIdHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public async Task<OperationResult<VacationRequest>> Handle(GetVacationRequestByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.VacationRequests.FirstOrDefaultAsync(v => v.Id == request.RequestId, cancellationToken);
            if (entity == null)
                return new OperationResult<VacationRequest> { Success = false, Message = "Vacation request not found" };

            return new OperationResult<VacationRequest> { Success = true, Data = entity, Message = "Vacation request retrieved successfully" };
        }
    }

}
