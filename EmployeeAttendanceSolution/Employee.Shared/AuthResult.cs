using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public class AuthResult
    {
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
        public UserInfo? User { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsSuccess => ErrorMessage == null;
    }
}
