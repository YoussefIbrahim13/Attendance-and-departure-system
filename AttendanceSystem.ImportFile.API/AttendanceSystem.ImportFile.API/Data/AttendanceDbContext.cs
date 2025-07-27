using Microsoft.EntityFrameworkCore;
using AttendanceSystem.ImportFile.API;
using AttendanceSystem.ImportFile.API.Shared;
namespace AttendanceSystem.ImportFile.API
{

    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Employee> Employees { get; set; } // Assuming you have an Employee entity

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendanceRecord>()
                .HasKey(ar => new { ar.EmployeeId, ar.Date });
        }
    }
}
