using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Comman
{
    public class RoleResult
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public Roles? RoleType { get; set; }
        public IEnumerable<IdentityError>? Errors { get; set; }
        public bool Success => Errors == null;
    }
}
