using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public class LoginResult
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
         public string? Token { get; set; }
       public DateTime? Expiration { get; set; }
        public object? User { get; set; }
    }
}
