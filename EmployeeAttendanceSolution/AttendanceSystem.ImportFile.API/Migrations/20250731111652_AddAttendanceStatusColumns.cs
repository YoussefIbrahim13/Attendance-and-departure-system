using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceSystem.ImportFile.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAttendanceStatusColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AttendanceRecords",
                newName: "PlannedStatus");

            migrationBuilder.AddColumn<int>(
                name: "ActualStatus",
                table: "AttendanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "AttendanceRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualStatus",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "PlannedStatus",
                table: "AttendanceRecords",
                newName: "Status");
        }
    }
}
