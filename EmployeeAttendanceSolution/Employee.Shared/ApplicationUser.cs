    using Microsoft.AspNetCore.Identity;
using System;

namespace EmployeesModels.Shared

{
    public class ApplicationUser: IdentityUser
    {
        
        public string Name { get; set; }
        public string? Department { get; set; }
        public string? Position { get; set; }
        public bool IsApproved { get; set; } = false; // Default to false, needs approval
                                                      
        // Navigation property for vacation requests
        public virtual ICollection<VacationRequest> VacationRequests { get; set; }


    }
}
