using Domain.Entities;
using EmployeesModels.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeesModels.Shared.Data
{
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

            // 🔹 FIXED: Correct relationship configuration
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Employee)
                .WithOne(e => e.User)
                .HasForeignKey<ApplicationUser>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Employee Configuration
            builder.Entity<Employee>()
                .HasKey(e => e.Id);

            builder.Entity<Employee>()
                .HasIndex(e => e.Code)
                .IsUnique();

            // Make sure Employee.Id is configured as string
            builder.Entity<Employee>()
                .Property(e => e.Id)
                .HasMaxLength(450); // Match Identity user ID length

            // Attendance Configuration
            builder.Entity<AttendanceRecord>()
                .HasKey(ar => new { ar.Code, ar.Date });

            // Vacation Requests Configuration
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
}
