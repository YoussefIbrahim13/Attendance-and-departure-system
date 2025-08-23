using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public class UserInfo
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Department { get; set; }
        public IList<string>? Roles { get; set; }
        public bool InRole(string role) =>
              Roles?.Contains(role, StringComparer.OrdinalIgnoreCase) ?? false;
    }
}
