using Domain.Enums;

namespace EmployeesModels.Shared
{
    ////////////////////////////////////
    public class EmployeeShare
    {
        public Guid Id { get; set; } // Primary Key
        public string Code { get; set; } = string.Empty; // Unique employee code
        public string Name { get; set; } = string.Empty;
        public DepartmentEnum Department { get; set; }
        public PositionEnum Position { get; set; }
         public string Email { get; set; }= string.Empty;
        public string Phone { get; set; }= string.Empty;
        public decimal Salary { get; set; }
        public string? ProfileImagePath { get; set; }

        // 🔹 Back reference (0..1 user)
        public virtual ApplicationUser? User { get; set; }

    }

}




