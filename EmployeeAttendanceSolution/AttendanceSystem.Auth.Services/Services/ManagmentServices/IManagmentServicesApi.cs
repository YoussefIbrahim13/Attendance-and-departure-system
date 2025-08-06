using EmployeesModels.Shared;

namespace AttendanceSystem.Auth.API.Services.Services.ManagmentServices
{
    public interface IManagmentServicesApi
    {
        Task<RoleResult> CreateRoleAsync(string roleName);
        Task<RoleResult> GetRoleAsync(string id);
        Task<IEnumerable<string>> GetAllRolesAsync();
        Task<OperationResult<List<UserResponseDto>>> GetAllUsersAsync();
        Task<UserResult> AddApplicationUserAsync(UserCreateDto dto, string roleName);
        Task<UserResult> GetApplicationUserAsync(string id);
        Task<UserResult> UpdateApplicationUserAsync(string id, UserUpdateDto dto);
        Task<OperationResult> DeleteApplicationUserAsync(string userId);
        Task<OperationResult> ApproveUserAsync(string userId);
        Task<IEnumerable<UserResponseDto>> GetPendingUsersAsync();
    }
}
