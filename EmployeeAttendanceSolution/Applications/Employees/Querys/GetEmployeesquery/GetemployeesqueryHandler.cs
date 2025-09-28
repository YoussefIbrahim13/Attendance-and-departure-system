using Applications.Employees.DTO.EmployeeDtos;
using AutoMapper;
using Infrastructure;
using Infrastructure.DBContext;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Employees.Querys.GetEmployeesquery;

public class GetemployeesqueryHandler : IRequestHandler<GetEmployeesquerys, List<EmployeeDto>>
{
     private readonly ApplicationDbContext _dbcontext;
     private readonly IMapper _mapper;

    public GetemployeesqueryHandler(ApplicationDbContext dbcontext, IMapper mapper)
    {
        _dbcontext = dbcontext;
        _mapper = mapper;
    }

    public async Task<List<EmployeeDto>> Handle(GetEmployeesquerys query, CancellationToken cancellationToken)
    {
        var employees = await _dbcontext.Employees.ToListAsync(cancellationToken);
        return _mapper.Map<List<EmployeeDto>>(employees);
    }
}