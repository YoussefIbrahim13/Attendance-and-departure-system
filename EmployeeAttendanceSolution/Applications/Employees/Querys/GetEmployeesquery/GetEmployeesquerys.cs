using Applications.Employees.DTO.EmployeeDtos;
using MediatR;

namespace Applications.Employees.Querys.GetEmployeesquery;

public class GetEmployeesquerys: IRequest<List<EmployeeDto>>
{
    
}