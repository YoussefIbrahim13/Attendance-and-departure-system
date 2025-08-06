using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeesModels.Shared
{
    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; set; }
    }
}
