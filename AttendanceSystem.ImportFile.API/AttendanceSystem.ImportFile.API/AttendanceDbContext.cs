using Microsoft.EntityFrameworkCore;
using AttendanceSystem.ImportFile.API;
namespace AttendanceSystem.ImportFile.API
{

    public class AttendanceDbContext : DbContext
    {
        public AttendanceDbContext(DbContextOptions<AttendanceDbContext> options) : base(options) { }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttendanceRecord>()
                .HasKey(ar => new { ar.EmployeeId, ar.Date });
        }
    }
}
