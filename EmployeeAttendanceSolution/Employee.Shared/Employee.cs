namespace EmployeesModels.Shared
{
    ////////////////////////////////////
    public class Employee
    {
        // 🔹 Fix: Change Guid to string to match ApplicationUser.EmployeeId
        public string Id { get; set; } = Guid.NewGuid().ToString(); // Primary Key
        public string Code { get; set; } = string.Empty; // Unique employee code
        public string Name { get; set; } = string.Empty;
        public DepartmentEnum Department { get; set; }
        public PositionEnum Position { get; set; }

        // 🔹 Back reference (0..1 user)
        public virtual ApplicationUser? User { get; set; }
    }

}




