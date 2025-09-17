using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations.ApplicationDb
{
    /// <inheritdoc />
    public partial class MergeAllDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceRecord",
                table: "AttendanceRecord");

            migrationBuilder.RenameTable(
                name: "AttendanceRecord",
                newName: "AttendanceRecords");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords",
                columns: new[] { "Code", "Date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AttendanceRecords",
                table: "AttendanceRecords");

            migrationBuilder.RenameTable(
                name: "AttendanceRecords",
                newName: "AttendanceRecord");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AttendanceRecord",
                table: "AttendanceRecord",
                columns: new[] { "Code", "Date" });
        }
    }
}
