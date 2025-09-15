using Applications.Employees.Commands.AddEmployees;
using Applications.Employees.Commands.UpdataEmployeecommand;
using Applications.Employees.profiles;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Applications.Employees.DTO.EmployeeDtos;

public class EmployeeeByCodeOutPut
{
    public EmployeeByCodeDto Data { get; set; }
}
public class EmployeeByCodeDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DepartmentEnum Department { get; set; }
    public PositionEnum Position { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string? ProfileImagePath { get; set; }
}
