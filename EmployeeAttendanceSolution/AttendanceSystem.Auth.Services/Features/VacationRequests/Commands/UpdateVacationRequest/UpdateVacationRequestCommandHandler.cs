using EmployeesModels.Shared;
using Infrastructure_.DBContext;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.UpdateVacationRequest
{
    public class UpdateVacationRequestCommandHandler : IRequestHandler<UpdateVacationRequestCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public UpdateVacationRequestCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(UpdateVacationRequestCommand request, CancellationToken cancellationToken)
        {
            var entity = await _dbContext.VacationRequests.FindAsync(request.RequestId);
            if (entity == null)
                return new OperationResult { Success = false, Message = "Vacation request not found" };

            if (request.Dto.FromTime >= request.Dto.ToTime)
                return new OperationResult { Success = false, Message = "End date must be after start date" };

            entity.FromTime = request.Dto.FromTime;
            entity.ToTime = request.Dto.ToTime;
            entity.Reason = request.Dto.Reason ?? "";

            // ✅ Recalculate DaysRequested (excluding Friday & Saturday)
            int daysRequested = 0;
            for (var date = entity.FromTime.Date; date <= entity.ToTime.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday)
                {
                    daysRequested++;
                }
            }
            entity.DaysRequested = daysRequested;

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new OperationResult { Success = true, Message = $"Vacation request updated successfully for {daysRequested} working days" };
        }

    }
}
