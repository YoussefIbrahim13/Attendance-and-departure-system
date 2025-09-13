using MediatR;

namespace Applications.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommand : IRequest<(bool Success, string Message)>
{
    public string Code { get; set; }

}