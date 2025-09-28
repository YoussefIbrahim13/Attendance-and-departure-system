//namespace AttendanceSystem.ImportFile.ui.Shared
//{
//    public enum AttendanceStatus
//    {
//        Present,
//        Absent,
//        Vacation,
//        WorkFromHome,
//        //Sick,
//        //Late,
//        //EarlyLeave
//    }


using Applications.Employees.Commands.UpdataEmployeecommand;
using AttendanceSystem.ImportFile.ui.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;


namespace AttendanceSystem.ImportFile.ui.Pages
{
    public partial class Employees
    {
        // Mapping departments to allowed positions
        private readonly Dictionary<DepartmentEnum, List<PositionEnum>> departmentPositions = new()
        {
            { DepartmentEnum.HR, new List<PositionEnum> { PositionEnum.Manager, PositionEnum.Recruiter } },
            { DepartmentEnum.IT, new List<PositionEnum> { PositionEnum.Manager, PositionEnum.Developer} },
            { DepartmentEnum.Finance, new List<PositionEnum> { PositionEnum.Manager, PositionEnum.Accountant } },
            { DepartmentEnum.Marketing, new List<PositionEnum> { PositionEnum.Manager, PositionEnum.Designer, PositionEnum.SalesRep } },
            { DepartmentEnum.Sales, new List<PositionEnum> { PositionEnum.Manager, PositionEnum.SalesRep } },
        };

        private IEnumerable<PositionEnum> GetPositionsForDepartment(DepartmentEnum? dept)
        {
            if (dept.HasValue && departmentPositions.TryGetValue(dept.Value, out var positions))
                return positions;
            return Enum.GetValues(typeof(PositionEnum)).Cast<PositionEnum>();
        }
        private List<EmployeeDto> employees = new();
        private string? searchcode;
        private DepartmentEnum? selectedDepartment = null;
        private PositionEnum? selectedPosition = null;
        private IEnumerable<EmployeeDto> filteredEmployees =>
            employees.Where(e =>
                (string.IsNullOrEmpty(searchcode) || e.Code == searchcode)
                && (!selectedDepartment.HasValue || e.Department == selectedDepartment.Value)
                && (!selectedPosition.HasValue || e.Position == selectedPosition.Value)
            );
        private EmployeeDto newEmployee = new EmployeeDto();
        private EmployeeDto? editRow;
        private string editName = string.Empty;
        private DepartmentEnum editDepartment;
        private PositionEnum editPosition;
        private string editEmail = string.Empty;
        private string editPhone = string.Empty;
        private decimal editSalary;


        [CascadingParameter] public Task<AuthenticationState> AuthenticationStateTask { get; set; }
        [Inject] NavigationManager Navigation { get; set; }

        protected override async Task OnInitializedAsync()
        {
            // جلب حالة المستخدم
            var authState = await AuthenticationStateTask;
            var user = authState.User;

            string[] roles = { "Admin" };

            if (!user.Identity.IsAuthenticated || !roles.Any(role => user.IsInRole(role)))
            {
                Navigation.NavigateTo("/");
                return;
            }

            // تحميل البيانات لو المستخدم عنده صلاحيات
            await LoadEmployees();
        }

        private async Task LoadEmployees()
        {
            employees = await AttendanceService.GetAllEmployeesAsync();
        }

        private async Task AddEmployee()
        {
            if (!string.IsNullOrWhiteSpace(newEmployee.Code) && !string.IsNullOrWhiteSpace(newEmployee.Name))
            {
                // تحقق من تكرار الـ Code محليًا قبل الإرسال للسيرفر
                if (employees.Any(e => e.Code == newEmployee.Code))
                {
                    Snackbar.Add("This Code already exists", Severity.Warning);
                    return;
                }

                try
                {
                    await AttendanceService.AddEmployeeAsync(newEmployee);
                    Snackbar.Add("Employee added successfully", Severity.Success);
                    newEmployee = new EmployeeDto();
                    await LoadEmployees();
                }
                catch (Exception ex)
                {
                    Snackbar.Add($"Failed to add employee: {ex.Message}", Severity.Error);
                }
            }
            else
            {
                Snackbar.Add("Please enter both Code and Name", Severity.Warning);
            }
        }


        private async Task DeleteEmployee(string id)
        {
            try
            {
                await AttendanceService.DeleteEmployeeAsync(id);
                Snackbar.Add("Employee deleted", Severity.Success);
                await LoadEmployees();
            }
            catch
            {
                Snackbar.Add("Failed to delete employee", Severity.Error);
            }
        }

        // إزالة تعريف EmployeeDto المحلي واستخدام النوع من AttendanceService فقط




        private void StartEdit(EmployeeDto emp)
        {
            editRow = emp;
            editName = emp.Name;
            editEmail = emp.Email;
            editPhone = emp.Phone;
            editSalary = emp.Salary;
            editDepartment = emp.Department;
            editPosition = emp.Position;
           
        }

        private void CancelEdit()
        {
            editRow = null;
        }
        private async Task SaveEdit(EmployeeDto emp)
        {
            if (editRow == null) return;

            var command = new UpdataEmployeecommand
            {
                Code = emp.Code,
                Name = editName,
                Email = editEmail,
                Phone = editPhone,
                Salary = editSalary,
                Department = editDepartment,
                Position = editPosition
            };

            var result = await AttendanceService.UpdateEmployeeAsync(command);

            if (result == "success")
            {
                Snackbar.Add("Employee updated successfully", Severity.Success);
                editRow = null;
                await LoadEmployees();
            }
            else
            {
                Snackbar.Add(result, Severity.Warning);
            }
        }




    }
}