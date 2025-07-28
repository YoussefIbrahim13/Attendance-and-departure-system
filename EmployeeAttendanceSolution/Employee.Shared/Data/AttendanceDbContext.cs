using EmployeesModels.Shared;
using Microsoft.EntityFrameworkCore;

namespace EmployeesModels.Shared.Data
{
    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendanceRecord>()
                .HasKey(ar => new { ar.EmployeeId, ar.Date });
        }
    }
}
