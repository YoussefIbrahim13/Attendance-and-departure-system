using System.ComponentModel.DataAnnotations;

namespace EmployeesModels.Shared
{
    public class UserUpdateDto
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Name { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public bool IsApproved { get; set; }
    }
}
