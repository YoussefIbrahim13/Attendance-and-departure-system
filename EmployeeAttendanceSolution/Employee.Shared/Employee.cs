namespace EmployeesModels.Shared
{
    ////////////////////////////////////
    public class Employee
    {
        public Guid Id { get; set; } // Primary Key
        public string Code { get; set; } = string.Empty; // Unique employee code
        public string Name { get; set; } = string.Empty;
        public DepartmentEnum Department { get; set; }
        public PositionEnum Position { get; set; }
    }

}




