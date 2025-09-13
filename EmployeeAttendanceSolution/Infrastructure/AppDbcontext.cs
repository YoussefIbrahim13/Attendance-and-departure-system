using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public class AppDbcontext :DbContext
{
    public AppDbcontext(DbContextOptions<AppDbcontext> options) : base(options) { }
    public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttendanceRecord>()
            .HasKey(ar => new { ar.Code, ar.Date });

        // Employee: Id as PK, Code as Unique
        modelBuilder.Entity<Employee>()
            .HasKey(e => e.Id);
        modelBuilder.Entity<Employee>()
            .HasIndex(e => e.Code)
            .IsUnique();
    }
}