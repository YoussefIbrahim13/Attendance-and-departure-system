using Infrastructure_;
using Infrastructure_.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler:IRequestHandler<DeleteEmployeeCommand, (bool Success, string Message)>
{
    private readonly ApplicationDbContext _db;

    public DeleteEmployeeCommandHandler(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<(bool Success, string Message)> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return (false, "Employee Code is required.");

        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Code == request.Code, cancellationToken);
        if (employee == null)
            return (false, "Employee not found.");

        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync(cancellationToken);

        return (true, "Employee deleted successfully.");
    }
}