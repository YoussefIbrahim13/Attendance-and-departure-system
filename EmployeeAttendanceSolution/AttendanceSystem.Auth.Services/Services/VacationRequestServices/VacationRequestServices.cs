using EmployeesModels.Shared;
using EmployeesModels.Shared.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AttendanceSystem.Auth.Services.Services.VacationRequestServices
{
    public class VacationRequestServices : IVacationRequestServices
    {
        private readonly ApplicationDbContext _dbContext;
        public VacationRequestServices(ApplicationDbContext dbContext)
        {
            _dbContext= dbContext ;
        }

        public async Task<OperationResult> CreateVacationRequestAsync(VacationRequest vacationRequest)
        {
           if (vacationRequest == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "Vacation request cannot be null"
                };
            }
           vacationRequest.Id = Guid.NewGuid().ToString();
            vacationRequest.Status = VacationRequestStatus.Pending;
            vacationRequest.CreatedAt = DateTime.UtcNow;
            _dbContext.VacationRequests.Add(vacationRequest);
            try
            {
                await _dbContext.SaveChangesAsync();
                return new OperationResult
                {
                    Success = true,
                    Message = "Vacation request created successfully"
                   
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = $"Error creating vacation request: {ex.Message}"
                };
            }

        }

        public async Task<OperationResult> DeleteVacationRequestAsync(string requestId)
        {
            var request = await _dbContext.VacationRequests.FindAsync(requestId);
            if (request == null)
                return new OperationResult { Success = false, Message = "Vacation request not found" };
            _dbContext.VacationRequests.Remove(request);
            await _dbContext.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Vacation request deleted successfully" };


        }
        public async Task<OperationResult> UpdateVacationRequestAsync(string requestId, VacationRequest vacationRequestDto)
        {
           var request = await _dbContext.VacationRequests.FindAsync(requestId);
           if (request == null)
            return new OperationResult { Success = false, Message = "Vacation request not found" };
           request.Status = vacationRequestDto.Status;
           request.FromTime = vacationRequestDto.FromTime;
           request.ToTime = vacationRequestDto.ToTime;
           request.Reason = vacationRequestDto.Reason;

           await _dbContext.SaveChangesAsync();
            return new OperationResult { Success = true, Message = "Vacation request updated successfully" };
        }

        public async Task<OperationResult<List<VacationRequest>>> GetAllVacationRequestsAsync()
        {
            var list = await _dbContext.VacationRequests
                                        .OrderByDescending(v => v.CreatedAt)
                                        .ToListAsync();

            return new OperationResult<List<VacationRequest>>
            {
                Success = true,
                Data = list,
                Message = "Vacation requests retrieved successfully"
            };

        }

        public async Task<OperationResult<VacationRequest>> GetVacationRequestByIdAsync(string requestId)
        {
            var request = await _dbContext.VacationRequests.FirstOrDefaultAsync(v => v.Id == requestId);

            if (request == null)
                return new OperationResult<VacationRequest>
                {
                    Success = false,
                    Message = "Vacation request not found"
                };

            return new OperationResult<VacationRequest>
            {
                Success = true,
                Data = request,
                Message = "Vacation request retrieved successfully"
            };
        }

        public async Task<OperationResult<List<VacationRequest>>> GetVacationRequestsByUserIdAsync(string userId)
        {
            var list = await _dbContext.VacationRequests
                                       .Where(v => v.UserId == userId)
                                       .OrderByDescending(v => v.CreatedAt)
                                       .ToListAsync();

            return new OperationResult<List<VacationRequest>>
            {
                Success = true,
                Data = list,
                Message = "Vacation requests for user retrieved successfully"
            };
        }

        public async Task<OperationResult> ApproveVacationRequestAsync(string requestId)
        {
            var request = await _dbContext.VacationRequests.FindAsync(requestId);
            if (request == null)
                return new OperationResult { Success = false, Message = "Vacation request not found" };

            request.Status = VacationRequestStatus.Approved;
            await _dbContext.SaveChangesAsync();

            return new OperationResult { Success = true, Message = "Vacation request approved" };
        }
        public async Task<OperationResult> RejectVacationRequestAsync(string requestId)
        {
            var request = await _dbContext.VacationRequests.FindAsync(requestId);
            if (request == null)
                return new OperationResult { Success = false, Message = "Vacation request not found" };

            request.Status = VacationRequestStatus.Rejected;
            await _dbContext.SaveChangesAsync();

            return new OperationResult { Success = true, Message = "Vacation request rejected" };
        }


    }
}
