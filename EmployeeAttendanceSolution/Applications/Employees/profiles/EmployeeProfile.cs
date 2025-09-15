using Applications.Employees.Commands.AddEmployees;
using Applications.Employees.Commands.UpdataEmployeecommand;
using Applications.Employees.DTO.EmployeeDtos;
using AutoMapper;
using Domain.Entities;

namespace Applications.Employees.profiles;


public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<Employee, EmployeeByCodeDto>();
        CreateMap<EmployeeByCodeDto, Employee>();
        CreateMap<Employee, EmployeeDto>();
        CreateMap<EmployeeDto, Employee>();
        CreateMap<AddEmployeesCommand, Employee>();
        CreateMap<UpdataEmployeecommand, Employee>()
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Employee, UpdataEmployeecommand>();

    }
}