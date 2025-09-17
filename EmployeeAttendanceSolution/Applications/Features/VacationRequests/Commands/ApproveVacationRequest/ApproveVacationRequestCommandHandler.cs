using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Infrastructure.DBContext;
using Domain.Comman;

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.ApproveVacationRequest
{

    public class ApproveVacationRequestCommandHandler : IRequestHandler<ApproveVacationRequestCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;
        public ApproveVacationRequestCommandHandler(ApplicationDbContext dbContext) => _dbContext = dbContext;

        public async Task<OperationResult> Handle(ApproveVacationRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.VacationRequests.FindAsync(request.RequestId);
            if (entity == null) return new OperationResult { Success = false, Message = "Vacation request not found" };

            entity.Status = VacationRequestStatus.Approved;
            entity.ApprovedBy = request.ApprovedBy;  
            entity.ApprovedAt = DateTime.UtcNow;     
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new OperationResult { Success = true, Message = "Vacation request approved" };
        }
    }
}
