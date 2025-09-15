using System.ComponentModel.DataAnnotations;

namespace EmployeesModels.Shared
{
    public class UserCreateDto
    {
        [Required] public string Email { get; set; }
        //[Required] public string Password { get; set; }
        [Required] public string Name { get; set; }
        // 🔹 Needed to link to Employee
        [Required] public string EmployeeCode { get; set; }
    }
}
