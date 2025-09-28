using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; }
        public bool IsApproved { get; set; } = false; // Default to false, needs approval
        public bool IsLockedByAdmin { get; set; } = false;

        // Navigation property for vacation requests
        public virtual ICollection<VacationRequest> VacationRequests { get; set; }

        // 🔹 Link to Employee
        //public string? EmployeeId { get; set; }  // Nullable FK
        public Guid? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

    }
}
