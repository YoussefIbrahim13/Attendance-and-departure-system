using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Comman
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public IEnumerable<IdentityError>? Errors { get; set; }
       
    }
    public class OperationResult<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public IEnumerable<IdentityError>? Errors { get; set; }
        public T? Data { get; set; }  // Add this generic Data property
    }
}
