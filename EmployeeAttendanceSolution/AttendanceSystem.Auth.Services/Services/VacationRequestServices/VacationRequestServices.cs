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
            _dbContext = dbContext;
        }

        public async Task<OperationResult> CreateVacationRequestAsync(string userId, CreateVacationRequestDto vacationRequestDto)
        {
            if (vacationRequestDto == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "Vacation request cannot be null"
                };
            }

            // Validate dates
            if (vacationRequestDto.FromTime >= vacationRequestDto.ToTime)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "End date must be after start date"
                };
            }

            // Create ENTITY from DTO (this is the key fix)
            var vacationRequest = new VacationRequest
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId, // Get userId from parameter, not DTO
                FromTime = vacationRequestDto.FromTime,
                ToTime = vacationRequestDto.ToTime,
                Reason = vacationRequestDto.Reason ?? "",
                Status = VacationRequestStatus.Pending, // Set status here
                CreatedAt = DateTime.UtcNow // Set timestamp here
            };

            _dbContext.VacationRequests.Add(vacationRequest); // Add ENTITY, not DTO

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

        public async Task<OperationResult> UpdateVacationRequestAsync(string requestId, UpdateVacationRequestDto vacationRequestDto)
        {
            var request = await _dbContext.VacationRequests.FindAsync(requestId);
            if (request == null)
                return new OperationResult { Success = false, Message = "Vacation request not found" };

            // Validate dates
            if (vacationRequestDto.FromTime >= vacationRequestDto.ToTime)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "End date must be after start date"
                };
            }

            // Update only allowed fields (don't update Status from DTO)
            request.FromTime = vacationRequestDto.FromTime;
            request.ToTime = vacationRequestDto.ToTime;
            request.Reason = vacationRequestDto.Reason ?? "";
            // Don't update Status - use separate Approve/Reject methods
            // Don't update CreatedAt or UserId

            try
            {
                await _dbContext.SaveChangesAsync();
                return new OperationResult { Success = true, Message = "Vacation request updated successfully" };
            }
            catch (Exception ex)
            {
                return new OperationResult { Success = false, Message = $"Error updating vacation request: {ex.Message}" };
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
