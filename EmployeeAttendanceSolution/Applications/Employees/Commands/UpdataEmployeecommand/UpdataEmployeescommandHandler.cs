using AutoMapper;
using Infrastructure;
using Infrastructure.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Employees.Commands.UpdataEmployeecommand;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdataEmployeecommand, (bool Success, string Message)>
{
    private readonly ApplicationDbContext _dbcontext;
    


    public UpdateEmployeeCommandHandler(ApplicationDbContext dbcontext )
    {
        _dbcontext = dbcontext;
        

    }

    public async Task<(bool Success, string Message)> Handle(UpdataEmployeecommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return (false, "Employee Code is required.");

        // البحث باستخدام Code بدل Id
        var employee = await _dbcontext.Employees
            .FirstOrDefaultAsync(e => e.Code == command.Code, cancellationToken);

        if (employee == null)
            return (false, "Employee not found.");

        // تحديث الحقول المطلوبة
        employee.Name = command.Name;
        employee.Email = command.Email;
        employee.Phone = command.Phone;
        employee.Salary = command.Salary;
        employee.Department = command.Department;
        employee.Position = command.Position;

        await _dbcontext.SaveChangesAsync(cancellationToken);

        return (true, "Employee updated successfully.");
    }



}