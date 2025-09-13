using Applications.Employees.DTO.EmployeeDtos;
using AutoMapper;
using Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Applications.Employees.Querys.GetEmployeesquery;

public class GetemployeesqueryHandler : IRequestHandler<GetEmployeesquerys, List<EmployeeDto>>
{
     private readonly AppDbcontext _dbcontext;
     private readonly IMapper _mapper;

    public GetemployeesqueryHandler(AppDbcontext dbcontext, IMapper mapper)
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