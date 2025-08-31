using EmployeesModels.Shared;
using EmployeesModels.Shared.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.VacationRequests.Commands.CreateVacationRequest
{
    public class CreateVacationRequestCommandHandler : IRequestHandler<CreateVacationRequestCommand, OperationResult>
    {
        private readonly ApplicationDbContext _dbContext;

        public CreateVacationRequestCommandHandler(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OperationResult> Handle(CreateVacationRequestCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Dto;

            if (dto.FromTime >= dto.ToTime)
                return new OperationResult { Success = false, Message = "End date must be after start date" };

            // Count working days (exclude Friday & Saturday)
            int daysRequested = 0;
            for (var date = dto.FromTime.Date; date <= dto.ToTime.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Friday && date.DayOfWeek != DayOfWeek.Saturday)
                {
                    daysRequested++;
                }
            }

            var vacationRequest = new VacationRequest
            {
                Id = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                FromTime = dto.FromTime,
                ToTime = dto.ToTime,
                Reason = dto.Reason ?? "",
                Status = VacationRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                DaysRequested = daysRequested
            };

            _dbContext.VacationRequests.Add(vacationRequest);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new OperationResult { Success = true, Message = $"Vacation request created successfully for {daysRequested} working days" };
        }
    }

}
