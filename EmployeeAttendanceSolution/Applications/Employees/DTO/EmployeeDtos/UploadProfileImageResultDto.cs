namespace Applications.Employees.DTO.EmployeeDtos;

public class UploadProfileImageResultDto
{

    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}