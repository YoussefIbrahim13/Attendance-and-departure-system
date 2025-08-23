using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public class UserResult
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public bool IsApproved { get; set; }
        public IEnumerable<string>? Roles { get; set; }
        public IEnumerable<IdentityError>? Errors { get; set; }
        public bool Success => Errors == null;


    }
}
