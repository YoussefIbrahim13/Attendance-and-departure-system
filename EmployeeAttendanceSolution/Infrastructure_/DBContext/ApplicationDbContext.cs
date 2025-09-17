using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure_.DBContext;

public class ApplicationDbContext
: IdentityDbContext<ApplicationUser, ApplicationRole, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // Identity + Business tables
    public DbSet<Employee> Employees { get; set; }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<VacationRequest> VacationRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Identity Table Renaming
        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<ApplicationRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

        // Convert enum RoleType to string in DB
        builder.Entity<ApplicationRole>()
               .Property(r => r.RoleType)
               .HasConversion<string>();

        // 🔹 User ↔ Employee (1-to-1)
        builder.Entity<ApplicationUser>()
        .HasOne(u => u.Employee)
        .WithOne(e => e.User)
        .HasForeignKey<ApplicationUser>(u => u.EmployeeId)
        .OnDelete(DeleteBehavior.Restrict)
        .IsRequired(false);  // explicitly say it's optional

        // 🔹 Employee Configuration
        builder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Salary).HasPrecision(18, 2);
        });

        // 🔹 AttendanceRecord Configuration
        builder.Entity<AttendanceRecord>(entity =>
        {
            entity.HasKey(ar => new { ar.Code, ar.Date });
        });

        // 🔹 VacationRequest Configuration
        builder.Entity<VacationRequest>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Status).HasConversion<string>();
            entity.HasOne(v => v.User)
                  .WithMany(u => u.VacationRequests)
                  .HasForeignKey(v => v.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

}
