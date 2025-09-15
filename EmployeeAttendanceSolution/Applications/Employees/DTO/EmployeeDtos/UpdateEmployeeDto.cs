using Domain.Enums;

namespace Applications.Employees.DTO.EmployeeDtos;

public class UpdateEmployeeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty; // Unique employee code
    public string Name { get; set; } = string.Empty;
    public DepartmentEnum Department { get; set; }
    public PositionEnum Position { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string? ProfileImagePath { get; set; }
}