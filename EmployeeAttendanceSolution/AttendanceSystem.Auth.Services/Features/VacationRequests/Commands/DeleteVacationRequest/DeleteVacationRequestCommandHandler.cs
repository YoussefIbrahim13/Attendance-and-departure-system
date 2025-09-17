using EmployeesModels.Shared;
using Infrastructure_.DBContext;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.DeleteVacationRequest
{
    public class DeleteVacationRequestCommandHandler : IRequestHandler<DeleteVacationRequestCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public DeleteVacationRequestCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(DeleteVacationRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.VacationRequests.FindAsync(request.RequestId);
            if (entity == null)
                return new OperationResult { Success = false, Message = "Vacation request not found" };

            _dbContext.VacationRequests.Remove(entity);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new OperationResult { Success = true, Message = "Vacation request deleted successfully" };
        }
    }
}
