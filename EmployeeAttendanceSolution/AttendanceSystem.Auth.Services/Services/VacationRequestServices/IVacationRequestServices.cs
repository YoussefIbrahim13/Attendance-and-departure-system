using EmployeesModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Services.VacationRequestServices
{
    public interface IVacationRequestServices
    {
        Task<OperationResult> CreateVacationRequestAsync(string userId, CreateVacationRequestDto vacationRequestDto);
        Task<OperationResult> UpdateVacationRequestAsync(string requestId, UpdateVacationRequestDto vacationRequestDto);
        Task<OperationResult> DeleteVacationRequestAsync(string requestId);
        Task<OperationResult<List<VacationRequest>>> GetAllVacationRequestsAsync();
        Task<OperationResult<VacationRequest>> GetVacationRequestByIdAsync(string requestId);
        Task<OperationResult<List<VacationRequest>>> GetVacationRequestsByUserIdAsync(string userId);
        Task<OperationResult> ApproveVacationRequestAsync(string requestId);
        Task<OperationResult> RejectVacationRequestAsync(string requestId);
    }
}
